namespace Parameters;

public interface IFactory<TProduct>
{
    IReadOnlyCollection<IParameter> Parameters { get; }
    TProduct Create(IReadOnlyDictionary<string, string> raw);
}