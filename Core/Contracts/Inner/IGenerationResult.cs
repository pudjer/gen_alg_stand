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
    /// Лучшее значение приспособленности <b>внутри этого поколения</b>,
    /// а не накопленный рекорд. Накопленный рекорд доступен как
    /// <see cref="IExecutionContext.Best"/> и
    /// <see cref="IGeneticAlgorithmResult.Best"/>.
    /// </summary>
    double BestFitness { get; }

    /// <summary>Среднее значение приспособленности в поколении.</summary>
    double AverageFitness { get; }
}
