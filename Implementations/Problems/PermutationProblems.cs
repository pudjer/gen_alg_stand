using Algorithms.Chromosomes;
using Core.Contracts;
using Implementations.Common;
using Implementations.Modules;
using Parameters;
using Setup;

namespace Implementations.Problems;

public abstract class PermutationProblemFactoryBase : IProblemFactory<PermutationChromosome>
{
    private static readonly Parameter<int> Length = Params.Int("length", 1);

    public IReadOnlyList<IParameter> Parameters { get; } = [Length];

    protected virtual int DefaultLength => 20;

    protected abstract IModuleFactory<ILossFunction<PermutationChromosome>> CreateLoss(int length);

    public IReadOnlyList<Variant<PermutationChromosome>> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        int length = values.Read(Length, DefaultLength);

        return new ProblemFactory<PermutationChromosome>
        {
            LossFunctions = [CreateLoss(length)],
            Selections = ModuleCatalog.Selections<PermutationChromosome>(),
            Crossovers = ModuleCatalog.PermutationCrossovers(),
            Mutations = ModuleCatalog.PermutationMutations(),
            InitialPopulations = [new RandomPermutationPopulationFactory(length)],
        }.Create(values);
    }
}

public sealed class TspProblemFactory : PermutationProblemFactoryBase
{
    protected override IModuleFactory<ILossFunction<PermutationChromosome>> CreateLoss(int length) =>
        new TspLossFactory<PermutationChromosome>(CreateDistances(length));

    private static double[,] CreateDistances(int count)
    {
        var random = new SeededRandom(1337 + count);
        var xs = new double[count];
        var ys = new double[count];

        for (int i = 0; i < count; i++)
        {
            xs[i] = random.NextDouble() * 100;
            ys[i] = random.NextDouble() * 100;
        }

        var distances = new double[count, count];
        for (int i = 0; i < count; i++)
        {
            for (int j = i + 1; j < count; j++)
            {
                double dx = xs[i] - xs[j];
                double dy = ys[i] - ys[j];
                double distance = Math.Sqrt((dx * dx) + (dy * dy));

                distances[i, j] = distance;
                distances[j, i] = distance;
            }
        }

        return distances;
    }
}

public sealed class NQueensProblemFactory : PermutationProblemFactoryBase
{
    protected override IModuleFactory<ILossFunction<PermutationChromosome>> CreateLoss(int length) =>
        new NQueensLossFactory<PermutationChromosome>();
}

public sealed class InversionCountProblemFactory : PermutationProblemFactoryBase
{
    protected override IModuleFactory<ILossFunction<PermutationChromosome>> CreateLoss(int length) =>
        new InversionCountLossFactory<PermutationChromosome>();
}