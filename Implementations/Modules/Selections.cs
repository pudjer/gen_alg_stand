using Algorithms.Selections;
using Core.Contracts;
using Implementations.Common;
using Parameters;
using Setup;

namespace Implementations.Modules;

public sealed class RandomSelectionFactory<TChromosome> : IModuleFactory<ISelection<TChromosome>>
    where TChromosome : IChromosome<TChromosome>
{
    public string Name => "Random";

    public IReadOnlyList<IParameter> Parameters { get; } = [];

    public ISelection<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new RandomSelection<TChromosome>();
    }
}

public sealed class RouletteWheelSelectionFactory<TChromosome> : IModuleFactory<ISelection<TChromosome>>
    where TChromosome : IChromosome<TChromosome>
{
    public string Name => "Roulette Wheel";

    public IReadOnlyList<IParameter> Parameters { get; } = [];

    public ISelection<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new RouletteWheelSelection<TChromosome>();
    }
}

public sealed class RankSelectionFactory<TChromosome> : IModuleFactory<ISelection<TChromosome>>
    where TChromosome : IChromosome<TChromosome>
{
    public string Name => "Rank";

    public IReadOnlyList<IParameter> Parameters { get; } = [];

    public ISelection<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new RankSelection<TChromosome>();
    }
}

public sealed class StochasticUniversalSamplingSelectionFactory<TChromosome>
    : IModuleFactory<ISelection<TChromosome>>
    where TChromosome : IChromosome<TChromosome>
{
    public string Name => "Stochastic Universal Sampling";

    public IReadOnlyList<IParameter> Parameters { get; } = [];

    public ISelection<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new StochasticUniversalSamplingSelection<TChromosome>();
    }
}

public sealed class TournamentSelectionFactory<TChromosome> : IModuleFactory<ISelection<TChromosome>>
    where TChromosome : IChromosome<TChromosome>
{
    private static readonly Parameter<int> TournamentSize = Params.Int("tournamentSize", 1);

    public string Name => "Tournament";

    public IReadOnlyList<IParameter> Parameters { get; } = [TournamentSize];

    public ISelection<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new TournamentSelection<TChromosome>(values.Read(TournamentSize, 3));
    }
}

public sealed class TruncationSelectionFactory<TChromosome> : IModuleFactory<ISelection<TChromosome>>
    where TChromosome : IChromosome<TChromosome>
{
    private static readonly Parameter<double> SurvivalRate = Params.Probability("survivalRate");

    public string Name => "Truncation";

    public IReadOnlyList<IParameter> Parameters { get; } = [SurvivalRate];

    public ISelection<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new TruncationSelection<TChromosome>(values.Read(SurvivalRate, 0.5));
    }
}