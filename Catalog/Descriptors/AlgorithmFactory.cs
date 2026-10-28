namespace Catalog.Descriptors;

public sealed class AlgorithmFactory
{
    private readonly Func<int, object> _create;

    public AlgorithmFactory(Func<int, object> create)
    {
        ArgumentNullException.ThrowIfNull(create);
        _create = create;
    }

    public object Create(int seed) => _create(seed);
}
