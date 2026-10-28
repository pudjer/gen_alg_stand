using Algorithms.Selections;
using Core.Contracts;
using Parameters;

namespace Catalog.Descriptors;

public static class SelectionDescriptors
{
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
