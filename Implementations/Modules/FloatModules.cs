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

public sealed class SphereLossFactory<TChromosome> : IModuleFactory<ILossFunction<TChromosome>>
    where TChromosome : IFloatChromosome<TChromosome>
{
    public string Name => "Sphere";

    public IReadOnlyList<IParameter> Parameters { get; } = [];

    public ILossFunction<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new SphereLossFunction<TChromosome>();
    }
}

public sealed class RastriginLossFactory<TChromosome> : IModuleFactory<ILossFunction<TChromosome>>
    where TChromosome : IFloatChromosome<TChromosome>
{
    public string Name => "Rastrigin";

    public IReadOnlyList<IParameter> Parameters { get; } = [];

    public ILossFunction<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new RastriginLossFunction<TChromosome>();
    }
}

public sealed class RosenbrockLossFactory<TChromosome> : IModuleFactory<ILossFunction<TChromosome>>
    where TChromosome : IFloatChromosome<TChromosome>
{
    public string Name => "Rosenbrock";

    public IReadOnlyList<IParameter> Parameters { get; } = [];

    public ILossFunction<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new RosenbrockLossFunction<TChromosome>();
    }
}

public sealed class AckleyLossFactory<TChromosome> : IModuleFactory<ILossFunction<TChromosome>>
    where TChromosome : IFloatChromosome<TChromosome>
{
    public string Name => "Ackley";

    public IReadOnlyList<IParameter> Parameters { get; } = [];

    public ILossFunction<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new AckleyLossFunction<TChromosome>();
    }
}

public sealed class GriewankLossFactory<TChromosome> : IModuleFactory<ILossFunction<TChromosome>>
    where TChromosome : IFloatChromosome<TChromosome>
{
    public string Name => "Griewank";

    public IReadOnlyList<IParameter> Parameters { get; } = [];

    public ILossFunction<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new GriewankLossFunction<TChromosome>();
    }
}

public sealed class SchwefelLossFactory<TChromosome> : IModuleFactory<ILossFunction<TChromosome>>
    where TChromosome : IFloatChromosome<TChromosome>
{
    public string Name => "Schwefel";

    public IReadOnlyList<IParameter> Parameters { get; } = [];

    public ILossFunction<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new SchwefelLossFunction<TChromosome>();
    }
}

public sealed class BoothLossFactory<TChromosome> : IModuleFactory<ILossFunction<TChromosome>>
    where TChromosome : IFloatChromosome<TChromosome>
{
    public string Name => "Booth";

    public IReadOnlyList<IParameter> Parameters { get; } = [];

    public ILossFunction<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new BoothLossFunction<TChromosome>();
    }
}

public sealed class BealeLossFactory<TChromosome> : IModuleFactory<ILossFunction<TChromosome>>
    where TChromosome : IFloatChromosome<TChromosome>
{
    public string Name => "Beale";

    public IReadOnlyList<IParameter> Parameters { get; } = [];

    public ILossFunction<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new BealeLossFunction<TChromosome>();
    }
}

public sealed class HimmelblauLossFactory<TChromosome> : IModuleFactory<ILossFunction<TChromosome>>
    where TChromosome : IFloatChromosome<TChromosome>
{
    public string Name => "Himmelblau";

    public IReadOnlyList<IParameter> Parameters { get; } = [];

    public ILossFunction<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new HimmelblauLossFunction<TChromosome>();
    }
}

public sealed class EasomLossFactory<TChromosome> : IModuleFactory<ILossFunction<TChromosome>>
    where TChromosome : IFloatChromosome<TChromosome>
{
    public string Name => "Easom";

    public IReadOnlyList<IParameter> Parameters { get; } = [];

    public ILossFunction<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new EasomLossFunction<TChromosome>();
    }
}

public sealed class BukinLossFactory<TChromosome> : IModuleFactory<ILossFunction<TChromosome>>
    where TChromosome : IFloatChromosome<TChromosome>
{
    public string Name => "Bukin";

    public IReadOnlyList<IParameter> Parameters { get; } = [];

    public ILossFunction<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new BukinLossFunction<TChromosome>();
    }
}

public sealed class MatyasLossFactory<TChromosome> : IModuleFactory<ILossFunction<TChromosome>>
    where TChromosome : IFloatChromosome<TChromosome>
{
    public string Name => "Matyas";

    public IReadOnlyList<IParameter> Parameters { get; } = [];

    public ILossFunction<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new MatyasLossFunction<TChromosome>();
    }
}

public sealed class TargetVectorLossFactory<TChromosome>(IReadOnlyList<double> target)
    : IModuleFactory<ILossFunction<TChromosome>>
    where TChromosome : IFloatChromosome<TChromosome>
{
    public string Name => "Target Vector";

    public IReadOnlyList<IParameter> Parameters { get; } = [];

    public ILossFunction<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new TargetVectorLossFunction<TChromosome>(target);
    }
}

public sealed class ArithmeticCrossoverFactory<TChromosome> : IModuleFactory<ICrossover<TChromosome>>
    where TChromosome : IFloatChromosome<TChromosome>
{
    private static readonly Parameter<double> Weight = Params.Probability("weight");
    private static readonly Parameter<double> CrossoverProbability = Params.Probability("crossoverProbability");

    public string Name => "Arithmetic";

    public IReadOnlyList<IParameter> Parameters { get; } = [Weight, CrossoverProbability];

    public ICrossover<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new ArithmeticCrossover<TChromosome>(
            values.Read(Weight, 0.5),
            values.Read(CrossoverProbability, 0.9));
    }
}

public sealed class BlendCrossoverFactory<TChromosome> : IModuleFactory<ICrossover<TChromosome>>
    where TChromosome : IFloatChromosome<TChromosome>
{
    private static readonly Parameter<double> Alpha = Params.Double("alpha", 0);
    private static readonly Parameter<double> CrossoverProbability = Params.Probability("crossoverProbability");

    public string Name => "Blend";

    public IReadOnlyList<IParameter> Parameters { get; } = [Alpha, CrossoverProbability];

    public ICrossover<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new BlendCrossover<TChromosome>(
            values.Read(Alpha, 0.5),
            values.Read(CrossoverProbability, 0.9));
    }
}

public sealed class GaussianMutationFactory<TChromosome> : IModuleFactory<IMutation<TChromosome>>
    where TChromosome : IFloatChromosome<TChromosome>
{
    private static readonly Parameter<double> GeneProbability = Params.Probability("geneProbability");
    private static readonly Parameter<double> Sigma = Params.Double("sigma", 0);
    private static readonly Parameter<double> ChromosomeProbability = Params.Probability("chromosomeProbability");

    public string Name => "Gaussian";

    public IReadOnlyList<IParameter> Parameters { get; } = [GeneProbability, Sigma, ChromosomeProbability];

    public IMutation<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new GaussianMutation<TChromosome>(
            values.Read(GeneProbability, 0.01),
            values.Read(Sigma, 0.1),
            values.Read(ChromosomeProbability, 0.9));
    }
}

public sealed class UniformFloatMutationFactory<TChromosome> : IModuleFactory<IMutation<TChromosome>>
    where TChromosome : IFloatChromosome<TChromosome>
{
    private static readonly Parameter<double> GeneProbability = Params.Probability("geneProbability");
    private static readonly Parameter<double> ChromosomeProbability = Params.Probability("chromosomeProbability");

    public string Name => "Uniform Float";

    public IReadOnlyList<IParameter> Parameters { get; } = [GeneProbability, ChromosomeProbability];

    public IMutation<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return new UniformFloatMutation<TChromosome>(
            values.Read(GeneProbability, 0.01),
            values.Read(ChromosomeProbability, 0.9));
    }
}

public sealed class RandomFloatPopulationFactory(int length, double minValue, double maxValue)
    : IModuleFactory<IInitialPopulation<FloatChromosome>>
{
    private static readonly Parameter<int> PopulationSize = Params.Int("populationSize", 1);

    public string Name => "Random Float";

    public IReadOnlyList<IParameter> Parameters { get; } = [PopulationSize];

    public IInitialPopulation<FloatChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        double[] genes = Enumerable.Repeat(minValue, length).ToArray();

        return new RandomFloatPopulation<FloatChromosome>(
            values.Read(PopulationSize, 100),
            new FloatChromosome(genes, minValue, maxValue));
    }
}