using Core.Contracts.Inner;
using Core.Contracts.Outer;

namespace Core.Algorithm;

/// <summary>Итог работы алгоритма.</summary>
internal sealed record GeneticAlgorithmResult<TChromosome>(
    TChromosome Best,
    double BestLoss,
    IReadOnlyList<IGenerationResult> History,
    TimeSpan Elapsed) : IGeneticAlgorithmResult<TChromosome>
    where TChromosome : IChromosome;