using Core.Contracts.Outer;

namespace Core.Contracts.Inner;

/// <summary>
/// Итог работы алгоритма — всё, что получает пользователь по завершении.
/// Внутренний API движка: тип создаёт сам алгоритм, читает вызывающий код.
/// </summary>
public interface IGeneticAlgorithmResult
{
    /// <summary>Лучшая найденная особь за весь прогон.</summary>
    Individual Best { get; }

    /// <summary>Сколько поколений выполнено. Производное от <see cref="History"/>.</summary>
    int Generations => History.Count;

    /// <summary>История по поколениям для графиков.</summary>
    IReadOnlyList<IGenerationResult> History { get; }


    /// <summary>Затраченное время.</summary>
    TimeSpan Elapsed { get; }
}
