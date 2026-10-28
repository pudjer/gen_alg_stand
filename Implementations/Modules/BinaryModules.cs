using Algorithms.Chromosomes;
using Algorithms.InitialPopulations;
using Algorithms.LossFunctions;
using Algorithms.Mutations;
using Core.Contracts;
using Implementations.Common;
using Parameters;
using Setup;

namespace Implementations.Modules;

public sealed class OneMaxLossFactory<TChromosome> : IModuleFactory<ILossFunction<TChromosome>>
    where TChromosome : IBinaryChromosome<TChromosome>
{
    public string Name => "OneMax";

    public IReadOnlyList<IParameter> Parameters { get; } = [];

    public ILossFunction<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new OneMaxLossFunction<TChromosome>();
    }
}

public sealed class LongestRunLossFactory<TChromosome> : IModuleFactory<ILossFunction<TChromosome>>
    where TChromosome : IBinaryChromosome<TChromosome>
{
    public string Name => "Longest Run";

    public IReadOnlyList<IParameter> Parameters { get; } = [];

    public ILossFunction<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new LongestRunLossFunction<TChromosome>();
    }
}

public sealed class KnapsackLossFactory<TChromosome>(int itemCount)
    : IModuleFactory<ILossFunction<TChromosome>>
    where TChromosome : IBinaryChromosome<TChromosome>
{
    public string Name => "Knapsack";

    public IReadOnlyList<IParameter> Parameters { get; } = [];

    public ILossFunction<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new KnapsackLossFunction<TChromosome>(itemCount);
    }
}

public sealed class BitFlipMutationFactory<TChromosome> : IModuleFactory<IMutation<TChromosome>>
    where TChromosome : IBinaryChromosome<TChromosome>
{
    private static readonly Parameter<double> GeneProbability = Params.Probability("geneProbability");
    private static readonly Parameter<double> ChromosomeProbability = Params.Probability("chromosomeProbability");

    public string Name => "Bit Flip";

    public IReadOnlyList<IParameter> Parameters { get; } = [GeneProbability, ChromosomeProbability];

    public IMutation<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new BitFlipMutation<TChromosome>(
            values.Read(GeneProbability, 0.01),
            values.Read(ChromosomeProbability, 0.9));
    }
}

public sealed class RandomBinaryPopulationFactory(int length)
    : IModuleFactory<IInitialPopulation<BinaryChromosome>>
{
    private static readonly Parameter<int> PopulationSize = Params.Int("populationSize", 1);
    private static readonly Parameter<double> TrueProbability = Params.Probability("trueProbability");

    public string Name => "Random Binary";

    public IReadOnlyList<IParameter> Parameters { get; } = [PopulationSize, TrueProbability];

    public IInitialPopulation<BinaryChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new RandomBinaryPopulation<BinaryChromosome>(
            values.Read(PopulationSize, 100),
            new BinaryChromosome(new bool[length]),
            values.Read(TrueProbability, 0.5));
    }
}