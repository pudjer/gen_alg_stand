using System.Collections;

namespace Parameters;

public sealed class ParameterSet : IEnumerable<KeyValuePair<string, string>>
{
    private readonly Dictionary<string, string> _values = new(StringComparer.Ordinal);

    public ParameterSet()
    {
    }

    public ParameterSet(IEnumerable<KeyValuePair<string, string>> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        foreach (var pair in values)
        {
            _values[pair.Key] = pair.Value;
        }
    }

    public string this[string key]
    {
        get => _values[key];
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            _values[key] = value;
        }
    }

    public T Get<T>(Parameter<T> parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter);
        return parameter.Parse(this[parameter.Key]);
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
