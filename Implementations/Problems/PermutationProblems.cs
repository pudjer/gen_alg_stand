using Algorithms.Chromosomes;
using Algorithms.LossFunctions;
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

    protected abstract IReadOnlyList<IModuleFactory<ILossFunction<PermutationChromosome>>> CreateLosses(
        int length);

    public IReadOnlyList<Variant<PermutationChromosome>> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);
        int length = values.Read(Length, DefaultLength);

        return new ProblemFactory<PermutationChromosome>
        {
            LossFunctions = CreateLosses(length),
            Selections = ModuleCatalog.Selections<PermutationChromosome>(),
            Crossovers = ModuleCatalog.PermutationCrossovers(),
            Mutations = ModuleCatalog.PermutationMutations(),
            InitialPopulations = [new RandomPermutationPopulationFactory(length)],
        }.Create(values);
    }
}

public sealed class TspProblemFactory(int seed = 1337) : PermutationProblemFactoryBase
{
    private readonly int _seed = seed;

    protected override IReadOnlyList<IModuleFactory<ILossFunction<PermutationChromosome>>> CreateLosses(
        int length) =>
    [
        new TspLossFactory<PermutationChromosome>(
            TspCityGenerator.Create(length, _seed + length, TspLayout.Random),
            "TSP (random)"),
        new TspLossFactory<PermutationChromosome>(
            TspCityGenerator.Create(length, _seed + length, TspLayout.Circle),
            "TSP (circle)"),
        new TspLossFactory<PermutationChromosome>(
            TspCityGenerator.Create(length, _seed + length, TspLayout.Line),
            "TSP (line)"),
    ];
}

public sealed class NQueensProblemFactory : PermutationProblemFactoryBase
{
    protected override IReadOnlyList<IModuleFactory<ILossFunction<PermutationChromosome>>> CreateLosses(
        int length) =>
    [
        new NQueensLossFactory<PermutationChromosome>(),
    ];
}

public sealed class InversionCountProblemFactory : PermutationProblemFactoryBase
{
    protected override IReadOnlyList<IModuleFactory<ILossFunction<PermutationChromosome>>> CreateLosses(
        int length) =>
    [
        new InversionCountLossFactory<PermutationChromosome>(),
    ];
}