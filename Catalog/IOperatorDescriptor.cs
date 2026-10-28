using Parameters;
using Studio;

namespace Catalog;

public interface IOperatorDescriptor
{
    string Key { get; }

    string DisplayName { get; }

    IReadOnlyList<ParameterSpec> Parameters { get; }
}

public interface IProblemEntry
{
    string Key { get; }

    string DisplayName { get; }

    IReadOnlyList<ParameterSpec> Parameters { get; }

    IReadOnlyList<IOperatorDescriptor> Crossovers { get; }

    IReadOnlyList<IOperatorDescriptor> Mutations { get; }

    IReadOnlyList<IOperatorDescriptor> Selections { get; }

    IReadOnlyList<IOperatorDescriptor> InitialPopulations { get; }

    IReadOnlyList<IOperatorDescriptor> Algorithms { get; }

    IExperimentRunner CreateRunner(ExperimentConfig config);
}
