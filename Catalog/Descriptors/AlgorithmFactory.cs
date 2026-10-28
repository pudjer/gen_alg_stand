namespace Catalog.Descriptors;

/// <summary>
/// A deferred algorithm construction: the runner supplies the seed once it knows the config, so the
/// random source is created exactly where the run starts.
/// </summary>
public sealed class AlgorithmFactory
{
    private readonly Func<int, object> _create;

    /// <summary>Initializes a new factory.</summary>
    /// <param name="create">Builds the algorithm from a seed.</param>
    public AlgorithmFactory(Func<int, object> create)
    {
        ArgumentNullException.ThrowIfNull(create);
        _create = create;
    }

    /// <summary>Creates the algorithm for the supplied seed.</summary>
    public object Create(int seed) => _create(seed);
}
