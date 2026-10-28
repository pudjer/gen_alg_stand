using Algorithms.Chromosomes;
using Core.Contracts;
using Implementations.Common;
using Implementations.Modules;
using Parameters;
using Setup;

namespace Implementations.Problems;

public abstract class BinaryProblemFactoryBase : IProblemFactory<BinaryChromosome>
{
    private static readonly Parameter<int> Length = Params.Int("length", 1);

    public IReadOnlyList<IParameter> Parameters { get; } = [Length];

    protected virtual int DefaultLength => 100;

    protected abstract IModuleFactory<ILossFunction<BinaryChromosome>> CreateLoss(int length);

    public IReadOnlyList<Variant<BinaryChromosome>> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        int length = values.Read(Length, DefaultLength);

        return new ProblemFactory<BinaryChromosome>
        {
            Algorithms = ModuleCatalog.Algorithms<BinaryChromosome>(),
            LossFunctions = [CreateLoss(length)],
            Selections = ModuleCatalog.Selections<BinaryChromosome>(),
            Crossovers = ModuleCatalog.BinaryCrossovers(),
            Mutations = ModuleCatalog.BinaryMutations(),
            InitialPopulations = [new RandomBinaryPopulationFactory(length)],
        }.Create(values);
    }
}

public sealed class OneMaxProblemFactory : BinaryProblemFactoryBase
{
    protected override IModuleFactory<ILossFunction<BinaryChromosome>> CreateLoss(int length) =>
        new OneMaxLossFactory<BinaryChromosome>();
}

public sealed class LongestRunProblemFactory : BinaryProblemFactoryBase
{
    protected override IModuleFactory<ILossFunction<BinaryChromosome>> CreateLoss(int length) =>
        new LongestRunLossFactory<BinaryChromosome>();
}

public sealed class KnapsackProblemFactory : BinaryProblemFactoryBase
{
    protected override int DefaultLength => 20;

    protected override IModuleFactory<ILossFunction<BinaryChromosome>> CreateLoss(int length) =>
        new KnapsackLossFactory<BinaryChromosome>(length);
}