using System.Globalization;

namespace Parameters;

public sealed class IntParameter : IParameter<int>
{
    public string Key { get; }
    public string DisplayName { get; }
    public string Default { get; }

    public IntParameter(string key, string displayName, string @default)
    {
        Key = key;
        DisplayName = displayName;
        Default = @default;
        _ = Parse(@default); // fail fast: Default обязан быть валидным
    }

    public int Parse(string raw) =>
        int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v)
            ? v
            : throw new FormatException($"Parameter '{Key}': '{raw}' is not a valid Int32.");

    public string Format(int value) => value.ToString(CultureInfo.InvariantCulture);
}

public sealed class DoubleParameter : IParameter<double>
{
    public string Key { get; }
    public string DisplayName { get; }
    public string Default { get; }

    public DoubleParameter(string key, string displayName, string @default)
    {
        Key = key;
        DisplayName = displayName;
        Default = @default;
        _ = Parse(@default);
    }

    public double Parse(string raw) =>
        double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var v)
            ? v
            : throw new FormatException($"Parameter '{Key}': '{raw}' is not a valid Double.");

    public string Format(double value) => value.ToString("R", CultureInfo.InvariantCulture);
}

public sealed class BoolParameter : IParameter<bool>
{
    public string Key { get; }
    public string DisplayName { get; }
    public string Default { get; }

    public BoolParameter(string key, string displayName, string @default)
    {
        Key = key;
        DisplayName = displayName;
        Default = @default;
        _ = Parse(@default);
    }

    public bool Parse(string raw) => raw switch
    {
        "true"  or "True"  or "1" => true,
        "false" or "False" or "0" => false,
        _ => throw new FormatException($"Parameter '{Key}': '{raw}' is not a valid Boolean.")
    };

    public string Format(bool value) => value ? "true" : "false";
}

public sealed class StringParameter : IParameter<string>
{
    public string Key { get; }
    public string DisplayName { get; }
    public string Default { get; }

    public StringParameter(string key, string displayName, string @default = "")
    {
        Key = key;
        DisplayName = displayName;
        Default = @default;
    }

    public string Parse(string raw) => raw;
    public string Format(string value) => value ?? string.Empty;
}