using Parameters;

namespace Catalog;

public sealed class OperatorDescriptor : IOperatorDescriptor
{
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

    public string Key { get; }

    public string DisplayName { get; }

    public IReadOnlyList<ParameterSpec> Parameters { get; }

    public Func<ParameterSet, object> Factory { get; }

    public static IReadOnlyList<ParameterSpec> Specs(params ParameterSpec[] specs) => specs;
}
