namespace Core.Contracts.Outer;

/// <summary>
/// Целевая функция задачи.
/// </summary>
/// <typeparam name="TChromosome">Конкретный тип хромосомы задачи.</typeparam>
public interface IFitnessFunction<TChromosome>
    where TChromosome : IChromosome
{
    /// <summary>
    /// Направление оптимизации. Задаётся функцией, а не прогоном:
    /// «минимизировать ошибку» — свойство задачи.
    /// </summary>
    bool IsMaximization { get; }

    /// <summary>
    /// Оценка хромосомы — чистая функция от хромосомы.
    /// Данные прогона движок не передаёт: если оценке нужны поколение,
    /// популяция или случайность, реализация сама доставляет их через
    /// собственные зависимости (конструктор).
    /// </summary>
    double Evaluate(TChromosome chromosome);
}
