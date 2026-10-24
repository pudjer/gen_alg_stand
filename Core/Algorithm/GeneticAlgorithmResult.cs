using Core.Contracts.Inner;
using Core.Contracts.Outer;

namespace Core.Algorithm;

/// <summary>Итог работы алгоритма.</summary>
internal sealed record GeneticAlgorithmResult(
    Individual Best,
    IReadOnlyList<IGenerationResult> History,
    TimeSpan Elapsed) : IGeneticAlgorithmResult;
