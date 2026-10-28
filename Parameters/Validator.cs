namespace Parameters;

public sealed class Validator<T> : IValidator<T>
{
    private readonly Func<string, T> _convert;

    public Validator(Func<string, T> convert) =>
        _convert = convert ?? throw new ArgumentNullException(nameof(convert));

    public T Validate(string value) => _convert(value);
}
