namespace Core.Contracts.Inner;

/// <summary>
/// Снимок одного завершённого поколения — точка на графике сходимости.
/// <para>
/// Inner-контракт: создаётся движком, реализовывать его не нужно.
/// </para>
/// </summary>
public interface IGenerationResult
{
    /// <summary>Номер поколения, начиная с 0.</summary>
    int Generation { get; }

    /// <summary>
    /// Лучшие (наименьшие) потери <b>внутри этого поколения</b>,
    /// а не накопленный рекорд. Накопленный рекорд доступен как
    /// <c>IExecutionContext&lt;TChromosome&gt;.BestLoss</c> и
    /// <c>IGeneticAlgorithmResult&lt;TChromosome&gt;.BestLoss</c>.
    /// </summary>
    double BestLoss { get; }

    /// <summary>Средние потери в поколении.</summary>
    double AverageLoss { get; }
}
