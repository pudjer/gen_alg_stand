using Algorithms.Selections;
using Core.Contracts;
using Parameters;

namespace Catalog.Descriptors;

/// <summary>Descriptors for selection operators.</summary>
/// <remarks>
/// Every selection needs the problem's loss function, so the descriptors are closed over both the
/// chromosome type and that loss function.
/// </remarks>
public static class SelectionDescriptors
{
    /// <summary>Gets the selection operators valid for <typeparamref name="TSelf"/>.</summary>
    /// <typeparam name="TSelf">Concrete chromosome type.</typeparam>
    /// <param name="lossFunction">Loss function used to rank individuals; lower is better.</param>
    /// <param name="keyPrefix">Key prefix used to build the operator keys.</param>
    /// <returns>Descriptors for random, rank, roulette-wheel, tournament, truncation and SUS selection.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="lossFunction"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="keyPrefix"/> is null or whitespace.</exception>
    public static IReadOnlyList<OperatorDescriptor> For<TSelf>(ILossFunction<TSelf> lossFunction, string keyPrefix = "selection")
        where TSelf : IChromosome<TSelf>
    {
        ArgumentNullException.ThrowIfNull(lossFunction);
        ArgumentException.ThrowIfNullOrWhiteSpace(keyPrefix);

        return
        [
            new($"{keyPrefix}.random", "Random selection", OperatorDescriptor.Specs(),
                _ => new RandomSelection<TSelf>()),
            new($"{keyPrefix}.rank", "Rank selection", OperatorDescriptor.Specs(),
                _ => new RankSelection<TSelf>(lossFunction)),
            new($"{keyPrefix}.roulettewheel", "Roulette-wheel selection", OperatorDescriptor.Specs(),
                _ => new RouletteWheelSelection<TSelf>(lossFunction)),
            new($"{keyPrefix}.tournament", "Tournament selection", OperatorDescriptor.Specs(
                    new ParameterSpec("tournamentSize", "Tournament size", ParameterKind.Int, "3", Min: 1, Max: 100, Step: 1)),
                p => new TournamentSelection<TSelf>(lossFunction, p.GetInt("tournamentSize"))),
            new($"{keyPrefix}.truncation", "Truncation selection", OperatorDescriptor.Specs(
                    new ParameterSpec("survivalRate", "Survival rate", ParameterKind.Double, "0.5", Min: 0.01, Max: 1, Step: 0.05)),
                p => new TruncationSelection<TSelf>(lossFunction, p.GetDouble("survivalRate"))),
            new($"{keyPrefix}.stochasticuniversalsampling", "Stochastic universal sampling", OperatorDescriptor.Specs(),
                _ => new StochasticUniversalSamplingSelection<TSelf>(lossFunction)),
        ];
    }
}
