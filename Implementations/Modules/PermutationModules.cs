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

public sealed class TspLossFactory<TChromosome> : IModuleFactory<ILossFunction<TChromosome>>
    where TChromosome : IPermutationChromosome<TChromosome>
{
    private readonly IReadOnlyList<TspCity> _cities;
    private readonly string _name;

    public TspLossFactory(IReadOnlyList<TspCity> cities, string name)
    {
        ArgumentNullException.ThrowIfNull(cities);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        _cities = cities;
        _name = name;
    }

    public string Name => _name;

    public IReadOnlyList<IParameter> Parameters { get; } = [];

    public ILossFunction<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new TspLossFunction<TChromosome>(_cities);
    }
}

public sealed class NQueensLossFactory<TChromosome> : IModuleFactory<ILossFunction<TChromosome>>
    where TChromosome : IPermutationChromosome<TChromosome>
{
    public string Name => "N-Queens";

    public IReadOnlyList<IParameter> Parameters { get; } = [];

    public ILossFunction<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new NQueensLossFunction<TChromosome>();
    }
}

public sealed class InversionCountLossFactory<TChromosome> : IModuleFactory<ILossFunction<TChromosome>>
    where TChromosome : IPermutationChromosome<TChromosome>
{
    public string Name => "Inversion Count";

    public IReadOnlyList<IParameter> Parameters { get; } = [];

    public ILossFunction<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new InversionCountLossFunction<TChromosome>();
    }
}

public sealed class OrderCrossoverFactory<TChromosome> : IModuleFactory<ICrossover<TChromosome>>
    where TChromosome : IOrderedChromosome<TChromosome>
{
    private static readonly Parameter<double> CrossoverProbability = Params.Probability("crossoverProbability");

    public string Name => "Order";

    public IReadOnlyList<IParameter> Parameters { get; } = [CrossoverProbability];

    public ICrossover<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new OrderCrossover<TChromosome>(values.Read(CrossoverProbability, 0.9));
    }
}

public sealed class CycleCrossoverFactory<TChromosome> : IModuleFactory<ICrossover<TChromosome>>
    where TChromosome : IOrderedChromosome<TChromosome>
{
    private static readonly Parameter<double> CrossoverProbability = Params.Probability("crossoverProbability");

    public string Name => "Cycle";

    public IReadOnlyList<IParameter> Parameters { get; } = [CrossoverProbability];

    public ICrossover<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new CycleCrossover<TChromosome>(values.Read(CrossoverProbability, 0.9));
    }
}

public sealed class PartiallyMappedCrossoverFactory<TChromosome> : IModuleFactory<ICrossover<TChromosome>>
    where TChromosome : IOrderedChromosome<TChromosome>
{
    private static readonly Parameter<double> CrossoverProbability = Params.Probability("crossoverProbability");

    public string Name => "Partially Mapped";

    public IReadOnlyList<IParameter> Parameters { get; } = [CrossoverProbability];

    public ICrossover<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new PartiallyMappedCrossover<TChromosome>(values.Read(CrossoverProbability, 0.9));
    }
}

public sealed class OrderBasedCrossoverFactory<TChromosome> : IModuleFactory<ICrossover<TChromosome>>
    where TChromosome : IOrderedChromosome<TChromosome>
{
    private static readonly Parameter<double> CrossoverProbability = Params.Probability("crossoverProbability");

    public string Name => "Order Based";

    public IReadOnlyList<IParameter> Parameters { get; } = [CrossoverProbability];

    public ICrossover<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new OrderBasedCrossover<TChromosome>(values.Read(CrossoverProbability, 0.9));
    }
}

public sealed class SwapMutationFactory<TChromosome> : IModuleFactory<IMutation<TChromosome>>
    where TChromosome : IOrderedChromosome<TChromosome>
{
    private static readonly Parameter<double> ChromosomeProbability = Params.Probability("chromosomeProbability");

    public string Name => "Swap";

    public IReadOnlyList<IParameter> Parameters { get; } = [ChromosomeProbability];

    public IMutation<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new SwapMutation<TChromosome>(values.Read(ChromosomeProbability, 0.9));
    }
}

public sealed class InsertMutationFactory<TChromosome> : IModuleFactory<IMutation<TChromosome>>
    where TChromosome : IOrderedChromosome<TChromosome>
{
    private static readonly Parameter<double> ChromosomeProbability = Params.Probability("chromosomeProbability");

    public string Name => "Insert";

    public IReadOnlyList<IParameter> Parameters { get; } = [ChromosomeProbability];

    public IMutation<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new InsertMutation<TChromosome>(values.Read(ChromosomeProbability, 0.9));
    }
}

public sealed class InversionMutationFactory<TChromosome> : IModuleFactory<IMutation<TChromosome>>
    where TChromosome : IOrderedChromosome<TChromosome>
{
    private static readonly Parameter<double> ChromosomeProbability = Params.Probability("chromosomeProbability");

    public string Name => "Inversion";

    public IReadOnlyList<IParameter> Parameters { get; } = [ChromosomeProbability];

    public IMutation<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new InversionMutation<TChromosome>(values.Read(ChromosomeProbability, 0.9));
    }
}

public sealed class ScrambleMutationFactory<TChromosome> : IModuleFactory<IMutation<TChromosome>>
    where TChromosome : IOrderedChromosome<TChromosome>
{
    private static readonly Parameter<double> ChromosomeProbability = Params.Probability("chromosomeProbability");

    public string Name => "Scramble";

    public IReadOnlyList<IParameter> Parameters { get; } = [ChromosomeProbability];

    public IMutation<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new ScrambleMutation<TChromosome>(values.Read(ChromosomeProbability, 0.9));
    }
}

public sealed class RandomPermutationPopulationFactory(int length)
    : IModuleFactory<IInitialPopulation<PermutationChromosome>>
{
    private static readonly Parameter<int> PopulationSize = Params.Int("populationSize", 1);

    public string Name => "Random Permutation";

    public IReadOnlyList<IParameter> Parameters { get; } = [PopulationSize];

    public IInitialPopulation<PermutationChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new RandomPermutationPopulation<PermutationChromosome>(
            values.Read(PopulationSize, 100),
            PermutationChromosome.Identity(length));
    }
}