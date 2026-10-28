using Algorithms.Chromosomes;
using Algorithms.Crossovers;
using Algorithms.InitialPopulations;
using Algorithms.LossFunctions;
using Algorithms.Mutations;
using Core.Contracts;
using Implementations.Common;
using Parameters;
using Setup;

namespace Implementations.Modules;

public sealed class SumTargetLossFactory<TChromosome>(long target)
    : IModuleFactory<ILossFunction<TChromosome>>
    where TChromosome : IIntChromosome<TChromosome>
{
    public string Name => "Sum Target";

    public IReadOnlyList<IParameter> Parameters { get; } = [];

    public ILossFunction<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new SumTargetLossFunction<TChromosome>(target);
    }
}

public sealed class IntegerArithmeticCrossoverFactory<TChromosome>
    : IModuleFactory<ICrossover<TChromosome>>
    where TChromosome : IIntChromosome<TChromosome>
{
    private static readonly Parameter<double> Weight = Params.Probability("weight");
    private static readonly Parameter<double> CrossoverProbability = Params.Probability("crossoverProbability");

    public string Name => "Integer Arithmetic";

    public IReadOnlyList<IParameter> Parameters { get; } = [Weight, CrossoverProbability];

    public ICrossover<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new IntegerArithmeticCrossover<TChromosome>(
            values.Read(Weight, 0.5),
            values.Read(CrossoverProbability, 0.9));
    }
}

public sealed class UniformIntMutationFactory<TChromosome> : IModuleFactory<IMutation<TChromosome>>
    where TChromosome : IIntChromosome<TChromosome>
{
    private static readonly Parameter<double> GeneProbability = Params.Probability("geneProbability");
    private static readonly Parameter<double> ChromosomeProbability = Params.Probability("chromosomeProbability");

    public string Name => "Uniform Int";

    public IReadOnlyList<IParameter> Parameters { get; } = [GeneProbability, ChromosomeProbability];

    public IMutation<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new UniformIntMutation<TChromosome>(
            values.Read(GeneProbability, 0.01),
            values.Read(ChromosomeProbability, 0.9));
    }
}

public sealed class RandomIntPopulationFactory(int length, int minValue, int maxValue)
    : IModuleFactory<IInitialPopulation<IntChromosome>>
{
    private static readonly Parameter<int> PopulationSize = Params.Int("populationSize", 1);

    public string Name => "Random Int";

    public IReadOnlyList<IParameter> Parameters { get; } = [PopulationSize];

    public IInitialPopulation<IntChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        int[] genes = Enumerable.Repeat(minValue, length).ToArray();

        return new RandomIntPopulation<IntChromosome>(
            values.Read(PopulationSize, 100),
            new IntChromosome(genes, minValue, maxValue));
    }
}