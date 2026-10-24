using Core.Contracts.Outer;

namespace Core.Contracts.Inner;

/// <summary>
/// Всё, что главный цикл передаёт операторам в момент вызова.
/// Единственный канал связи между движком и стратегиями:
/// сами стратегии состояния прогона не хранят.
/// <para>
/// Inner-контракт: предоставляется движком, реализовывать его не нужно.
/// </para>
/// </summary>
/// <typeparam name="TChromosome">Конкретный тип хромосомы задачи.</typeparam>
public interface IExecutionContext<TChromosome>
    where TChromosome : IChromosome
{
    /// <summary>
    /// Задача, частью которой является вызываемый компонент.
    /// Развязывает компоненты: соседи и целевая функция видны отсюда,
    /// а не через собственный конструктор. Направление оптимизации
    /// читается там же — <c>Problem.FitnessFunction.IsMaximization</c>.
    /// </summary>
    IGeneticProblem<TChromosome> Problem { get; }

    /// <summary>Номер текущего поколения, начиная с 0.</summary>
    int Generation { get; }

    /// <summary>
    /// Популяция, с которой работает текущий вызов. Пуста только внутри
    /// создания первого поколения — к первому вызову стратегии
    /// движок её уже заполнил.
    /// </summary>
    IPopulation<TChromosome> Population { get; }

    /// <summary>
    /// Лучшая хромосома за весь прогон. <c>null</c> только внутри создания
    /// первого поколения, пока не оценена ни одна хромосома.
    /// </summary>
    TChromosome? Best { get; }

    /// <summary>
    /// Оценка <see cref="Best"/>. <c>null</c> ровно тогда, когда <c>null</c>
    /// и <see cref="Best"/>: движок обновляет оба значения одним присваиванием
    /// на границе поколения, поэтому состояние «хромосома есть, а оценки нет»
    /// недостижимо.
    /// </summary>
    double? BestFitness { get; }

    /// <summary>
    /// История завершённых поколений. Живое представление:
    /// после возврата из вызова список продолжит пополняться.
    /// </summary>
    IReadOnlyList<IGenerationResult> History { get; }

    /// <summary>Источник случайных чисел прогона.</summary>
    IRandom Random { get; }

    /// <summary>Признак запрошенной отмены.</summary>
    CancellationToken CancellationToken { get; }
}