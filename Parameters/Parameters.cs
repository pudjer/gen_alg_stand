namespace Parameters;

public interface IParameter
{
    string Key { get; }
    string DisplayName { get; }
    string Default { get; }
}

public interface IParameter<T> : IParameter
{
    T Parse(string raw);
    string Format(T value);
}


public sealed class ParameterSet
{
    private readonly Dictionary<string, IParameter> _all = new();

    public IParameter<T> Add<T>(IParameter<T> p)
    {
        if (!_all.TryAdd(p.Key, p))
            throw new ArgumentException($"Duplicate parameter key '{p.Key}'.");
        return p;
    }

    public IReadOnlyCollection<IParameter> All => _all.Values;

    public T Parse<T>(string key, string raw) =>
        ((IParameter<T>)_all[key]).Parse(raw);
}