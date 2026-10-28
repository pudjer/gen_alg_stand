namespace Parameters;

public interface IValidator<out T>
{
    T Validate(string value);
}

public sealed class Parameter<T>(string key, IValidator<T> validator) : IParameter
{
    public string Key { get; } = !string.IsNullOrWhiteSpace(key)
        ? key
        : throw new ArgumentException("The key must not be blank.", nameof(key));

    public IValidator<T> Validator { get; } = validator ?? throw new ArgumentNullException(nameof(validator));

    public T Parse(string value) => Validator.Validate(value);

    public override string ToString() => Key;
}
