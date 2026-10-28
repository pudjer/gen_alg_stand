using Parameters;
using Studio;

namespace Catalog;

/// <summary>
/// A selectable operator: something the user can pick and configure.
/// Implementations are generic over the chromosome type; consumers only see this interface.
/// </summary>
public interface IOperatorDescriptor
{
    /// <summary>Gets the stable key, for example <c>"crossover.singlepoint"</c>.</summary>
    string Key { get; }

    /// <summary>Gets the human readable name.</summary>
    string DisplayName { get; }

    /// <summary>Gets the tunable parameters of this operator.</summary>
    IReadOnlyList<ParameterSpec> Parameters { get; }
}

/// <summary>
/// A problem the bench can run: it knows which operators are compatible with it and can build
/// a runnable experiment from a configuration.
/// </summary>
public interface IProblemEntry
{
    /// <summary>Gets the stable key, for example <c>"problem.onemax"</c>.</summary>
    string Key { get; }

    /// <summary>Gets the human readable name.</summary>
    string DisplayName { get; }

    /// <summary>Gets the parameters describing the problem itself (size, bounds, target, ...).</summary>
    IReadOnlyList<ParameterSpec> Parameters { get; }

    /// <summary>Gets the crossovers that can be used with this problem.</summary>
    IReadOnlyList<IOperatorDescriptor> Crossovers { get; }

    /// <summary>Gets the mutations that can be used with this problem.</summary>
    IReadOnlyList<IOperatorDescriptor> Mutations { get; }

    /// <summary>Gets the selections that can be used with this problem.</summary>
    IReadOnlyList<IOperatorDescriptor> Selections { get; }

    /// <summary>Gets the initial population generators that can be used with this problem.</summary>
    IReadOnlyList<IOperatorDescriptor> InitialPopulations { get; }

    /// <summary>Gets the algorithms that can drive this problem.</summary>
    IReadOnlyList<IOperatorDescriptor> Algorithms { get; }

    /// <summary>
    /// Builds a runner for <paramref name="config"/>. This is the single place where the
    /// chromosome type parameter is resolved at runtime.
    /// </summary>
    /// <param name="config">The configuration to run.</param>
    /// <returns>A runner bound to the concrete chromosome type of this problem.</returns>
    /// <exception cref="ArgumentException">The config names an unknown operator or an invalid value.</exception>
    IExperimentRunner CreateRunner(ExperimentConfig config);
}
