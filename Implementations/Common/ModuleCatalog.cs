using Algorithms.Chromosomes;
using Core.Contracts;
using Implementations.Modules;
using Setup;

namespace Implementations.Common;

internal static class ModuleCatalog
{
    internal static IReadOnlyList<IModuleFactory<ISelection<TChromosome>>> Selections<TChromosome>()
        where TChromosome : IChromosome<TChromosome> =>
    [
        new TournamentSelectionFactory<TChromosome>(),
        new RouletteWheelSelectionFactory<TChromosome>(),
        new RankSelectionFactory<TChromosome>(),
        new StochasticUniversalSamplingSelectionFactory<TChromosome>(),
        new TruncationSelectionFactory<TChromosome>(),
        new RandomSelectionFactory<TChromosome>(),
    ];

    internal static IReadOnlyList<IModuleFactory<ICrossover<BinaryChromosome>>> BinaryCrossovers() =>
    [
        new SinglePointCrossoverFactory<BinaryChromosome, bool>(),
        new TwoPointCrossoverFactory<BinaryChromosome, bool>(),
        new UniformCrossoverFactory<BinaryChromosome, bool>(),
    ];

    internal static IReadOnlyList<IModuleFactory<IMutation<BinaryChromosome>>> BinaryMutations() =>
    [
        new BitFlipMutationFactory<BinaryChromosome>(),
    ];

    internal static IReadOnlyList<IModuleFactory<ICrossover<PermutationChromosome>>> PermutationCrossovers() =>
    [
        new OrderCrossoverFactory<PermutationChromosome>(),
        new CycleCrossoverFactory<PermutationChromosome>(),
        new PartiallyMappedCrossoverFactory<PermutationChromosome>(),
        new OrderBasedCrossoverFactory<PermutationChromosome>(),
    ];

    internal static IReadOnlyList<IModuleFactory<IMutation<PermutationChromosome>>> PermutationMutations() =>
    [
        new SwapMutationFactory<PermutationChromosome>(),
        new InsertMutationFactory<PermutationChromosome>(),
        new InversionMutationFactory<PermutationChromosome>(),
        new ScrambleMutationFactory<PermutationChromosome>(),
    ];

    internal static IReadOnlyList<IModuleFactory<ICrossover<FloatChromosome>>> FloatCrossovers() =>
    [
        new ArithmeticCrossoverFactory<FloatChromosome>(),
        new BlendCrossoverFactory<FloatChromosome>(),
        new UniformCrossoverFactory<FloatChromosome, double>(),
    ];

    internal static IReadOnlyList<IModuleFactory<IMutation<FloatChromosome>>> FloatMutations() =>
    [
        new GaussianMutationFactory<FloatChromosome>(),
        new UniformFloatMutationFactory<FloatChromosome>(),
    ];

    internal static IReadOnlyList<IModuleFactory<ICrossover<IntChromosome>>> IntCrossovers() =>
    [
        new IntegerArithmeticCrossoverFactory<IntChromosome>(),
        new UniformCrossoverFactory<IntChromosome, int>(),
        new SinglePointCrossoverFactory<IntChromosome, int>(),
    ];

    internal static IReadOnlyList<IModuleFactory<IMutation<IntChromosome>>> IntMutations() =>
    [
        new UniformIntMutationFactory<IntChromosome>(),
    ];
}