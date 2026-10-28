using System.Globalization;

namespace Parameters;

/// <summary>
/// A validated set of parameter values keyed by <see cref="ParameterSpec.Key"/>.
/// Values are stored as strings so a set can be serialized without knowing the concrete types.
/// </summary>
public sealed class ParameterSet
{
    private readonly Dictionary<string, string> _values;

    private ParameterSet(Dictionary<string, string> values) => _values = values;

    /// <summary>Gets the raw values, keyed by parameter key.</summary>
    public IReadOnlyDictionary<string, string> Values => _values;

    /// <summary>Creates a set filled with the defaults of <paramref name="specs"/>.</summary>
    /// <param name="specs">The specs describing the parameters.</param>
    /// <returns>A set where every key maps to its default.</returns>
    public static ParameterSet FromDefaults(IEnumerable<ParameterSpec> specs)
    {
        ArgumentNullException.ThrowIfNull(specs);

        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var spec in specs)
        {
            values[spec.Key] = spec.Default;
        }

        return new ParameterSet(values);
    }

    /// <summary>
    /// Creates a set from raw values, filling any missing key with its default and rejecting
    /// unknown keys or values that fail validation.
    /// </summary>
    /// <param name="specs">The specs describing the parameters.</param>
    /// <param name="raw">Raw values; keys not present fall back to the spec default.</param>
    /// <returns>A validated set.</returns>
    /// <exception cref="ArgumentException">A key is unknown or a value is invalid.</exception>
    public static ParameterSet Create(IEnumerable<ParameterSpec> specs, IReadOnlyDictionary<string, string>? raw)
    {
        ArgumentNullException.ThrowIfNull(specs);

        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        var known = new HashSet<string>(StringComparer.Ordinal);

        foreach (var spec in specs)
        {
            known.Add(spec.Key);
            values[spec.Key] = raw is not null && raw.TryGetValue(spec.Key, out var provided)
                ? provided
                : spec.Default;

            Validate(spec, values[spec.Key]);
        }

        if (raw is not null)
        {
            foreach (var key in raw.Keys)
            {
                if (!known.Contains(key))
                {
                    throw new ArgumentException($"Unknown parameter '{key}'.", nameof(raw));
                }
            }
        }

        return new ParameterSet(values);
    }

    /// <summary>Returns a copy of this set with one value replaced and validated.</summary>
    /// <param name="spec">The spec of the parameter to change.</param>
    /// <param name="value">The new raw value.</param>
    /// <returns>A new set; this instance is unchanged.</returns>
    public ParameterSet With(ParameterSpec spec, string value)
    {
        ArgumentNullException.ThrowIfNull(spec);
        Validate(spec, value);

        var copy = new Dictionary<string, string>(_values, StringComparer.Ordinal) { [spec.Key] = value };
        return new ParameterSet(copy);
    }

    /// <summary>Reads an integer parameter.</summary>
    /// <param name="key">Parameter key.</param>
    /// <returns>The parsed value.</returns>
    public int GetInt(string key) => int.Parse(Get(key), CultureInfo.InvariantCulture);

    /// <summary>Reads a double parameter.</summary>
    /// <param name="key">Parameter key.</param>
    /// <returns>The parsed value.</returns>
    public double GetDouble(string key) => double.Parse(Get(key), CultureInfo.InvariantCulture);

    /// <summary>Reads a boolean parameter.</summary>
    /// <param name="key">Parameter key.</param>
    /// <returns>The parsed value.</returns>
    public bool GetBool(string key) => bool.Parse(Get(key));

    /// <summary>Reads a text or choice parameter.</summary>
    /// <param name="key">Parameter key.</param>
    /// <returns>The raw value.</returns>
    public string GetText(string key) => Get(key);

    private string Get(string key) =>
        _values.TryGetValue(key, out var value)
            ? value
            : throw new KeyNotFoundException($"Parameter '{key}' is not part of this set.");

    private static void Validate(ParameterSpec spec, string value)
    {
        switch (spec.Kind)
        {
            case ParameterKind.Int:
                if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i))
                {
                    throw new ArgumentException($"Parameter '{spec.Key}': '{value}' is not a valid integer.");
                }

                CheckRange(spec, i);
                break;

            case ParameterKind.Double:
                if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var d)
                    || !double.IsFinite(d))
                {
                    throw new ArgumentException($"Parameter '{spec.Key}': '{value}' is not a valid finite number.");
                }

                CheckRange(spec, d);
                break;

            case ParameterKind.Bool:
                if (!bool.TryParse(value, out _))
                {
                    throw new ArgumentException($"Parameter '{spec.Key}': '{value}' is not a valid boolean.");
                }

                break;

            case ParameterKind.Choice:
                if (spec.Options is null || !spec.Options.Contains(value, StringComparer.Ordinal))
                {
                    throw new ArgumentException($"Parameter '{spec.Key}': '{value}' is not one of the allowed options.");
                }

                break;

            case ParameterKind.Text:
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(spec), spec.Kind, "Unknown parameter kind.");
        }
    }

    private static void CheckRange(ParameterSpec spec, double value)
    {
        if (spec.Min is { } min && value < min)
        {
            throw new ArgumentException($"Parameter '{spec.Key}': {value} is below the minimum {min}.");
        }

        if (spec.Max is { } max && value > max)
        {
            throw new ArgumentException($"Parameter '{spec.Key}': {value} is above the maximum {max}.");
        }
    }
}
