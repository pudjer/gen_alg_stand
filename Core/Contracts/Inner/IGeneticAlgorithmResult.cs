using Core.Contracts.Outer;

namespace Core.Contracts.Inner;

/// <summary>
/// Итог работы алгоритма — всё, что получает пользователь по завершении.
/// <para>
/// Inner-контракт: создаётся движком, реализовывать его не нужно.
/// </para>
/// </summary>
/// <typeparam name="TChromosome">Конкретный тип хромосомы задачи.</typeparam>
public interface IGeneticAlgorithmResult<TChromosome>
    where TChromosome : IChromosome
{
    /// <summary>Лучшая хромосома за весь прогон.</summary>
    TChromosome Best { get; }

    /// <summary>
    /// Потери <see cref="Best"/>. Непусты всегда: популяция непуста,
    /// значит лучшая хромосома в итоге есть.
    /// </summary>
    double BestLoss { get; }

    /// <summary>Сколько поколений выполнено. Производное от <see cref="History"/>.</summary>
    int Generations => History.Count;

    /// <summary>История по поколениям для графиков.</summary>
    IReadOnlyList<IGenerationResult> History { get; }

    /// <summary>Затраченное время.</summary>
    TimeSpan Elapsed { get; }
}