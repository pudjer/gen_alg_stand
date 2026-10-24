using Core.Contracts.Inner;

namespace Core.Algorithm;

/// <summary>Снимок одного поколения — точка на графике сходимости.</summary>
internal sealed record GenerationResult(int Generation, double BestLoss, double AverageLoss)
    : IGenerationResult;
