using Parameters;

namespace Catalog;

/// <summary>
/// A concrete operator descriptor. The factory is intentionally kept inside Catalog so that
/// generic chromosome types never leak to consumers of the catalog.
/// </summary>
public sealed class OperatorDescriptor : IOperatorDescriptor
{
    /// <summary>Initializes a new descriptor.</summary>
    /// <param name="key">Stable key, for example <c>"crossover.singlepoint"</c>.</param>
    /// <param name="displayName">Human readable name.</param>
    /// <param name="parameters">Tunable parameters of the operator.</param>
    /// <param name="factory">Builds the concrete operator from a validated parameter set.</param>
    /// <exception cref="ArgumentException"><paramref name="key"/> or <paramref name="displayName"/> is null or whitespace.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="parameters"/> or <paramref name="factory"/> is <see langword="null"/>.</exception>
    public OperatorDescriptor(
        string key,
        string displayName,
        IReadOnlyList<ParameterSpec> parameters,
        Func<ParameterSet, object> factory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        ArgumentNullException.ThrowIfNull(parameters);
        ArgumentNullException.ThrowIfNull(factory);

        Key = key;
        DisplayName = displayName;
        Parameters = parameters;
        Factory = factory;
    }

    /// <inheritdoc />
    public string Key { get; }

    /// <inheritdoc />
    public string DisplayName { get; }

    /// <inheritdoc />
    public IReadOnlyList<ParameterSpec> Parameters { get; }

    /// <summary>Gets the factory that creates the concrete operator from validated parameters.</summary>
    public Func<ParameterSet, object> Factory { get; }

    /// <summary>Builds a concise parameter specification list.</summary>
    public static IReadOnlyList<ParameterSpec> Specs(params ParameterSpec[] specs) => specs;
}
