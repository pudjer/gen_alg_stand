using Algorithms.Chromosomes;
using Algorithms.Crossovers;
using Core.Contracts;
using Implementations.Common;
using Parameters;
using Setup;

namespace Implementations.Modules;

public sealed class SinglePointCrossoverFactory<TChromosome, TGene>
    : IModuleFactory<ICrossover<TChromosome>>
    where TChromosome : IGenotypeChromosome<TChromosome, TGene>
{
    private static readonly Parameter<double> CrossoverProbability = Params.Probability("crossoverProbability");

    public string Name => "Single Point";

    public IReadOnlyList<IParameter> Parameters { get; } = [CrossoverProbability];

    public ICrossover<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new SinglePointCrossover<TChromosome, TGene>(values.Read(CrossoverProbability, 0.9));
    }
}

public sealed class TwoPointCrossoverFactory<TChromosome, TGene>
    : IModuleFactory<ICrossover<TChromosome>>
    where TChromosome : IGenotypeChromosome<TChromosome, TGene>
{
    private static readonly Parameter<double> CrossoverProbability = Params.Probability("crossoverProbability");

    public string Name => "Two Point";

    public IReadOnlyList<IParameter> Parameters { get; } = [CrossoverProbability];

    public ICrossover<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new TwoPointCrossover<TChromosome, TGene>(values.Read(CrossoverProbability, 0.9));
    }
}

public sealed class UniformCrossoverFactory<TChromosome, TGene>
    : IModuleFactory<ICrossover<TChromosome>>
    where TChromosome : IGenotypeChromosome<TChromosome, TGene>
{
    private static readonly Parameter<double> SwapProbability = Params.Probability("swapProbability");
    private static readonly Parameter<double> CrossoverProbability = Params.Probability("crossoverProbability");

    public string Name => "Uniform";

    public IReadOnlyList<IParameter> Parameters { get; } = [SwapProbability, CrossoverProbability];

    public ICrossover<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new UniformCrossover<TChromosome, TGene>(
            values.Read(SwapProbability, 0.5),
            values.Read(CrossoverProbability, 0.9));
    }
}