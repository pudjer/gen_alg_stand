using Core.Contracts.Outer;

namespace Core.Contracts.Inner;

/// <summary>
/// Внутренний API движка: единственный тип, который нужен вызывающему коду,
/// чтобы запустить алгоритм.
/// <para>
/// Политика поколения, зашитая в движок: лучшая особь родительской популяции
/// всегда переходит в следующее поколение без изменений — ни скрещивание,
/// ни мутация её не касаются. Элитизм не настраивается: это инвариант движка,
/// а не параметр задачи. Популяция из одной особи — исключение: слот отдаётся
/// потомку, иначе прогон застыл бы навсегда.
/// </para>
/// </summary>
/// <typeparam name="TChromosome">Конкретный тип хромосомы задачи.</typeparam>
public interface IGeneticAlgorithm<TChromosome>
    where TChromosome : IChromosome
{
    /// <summary>
    /// Запускает прогон над задачей с тем же типом хромосомы.
    /// </summary>
    /// <param name="problem">Задача: доменные компоненты без настроек прогона.</param>
    /// <param name="options">
    /// Настройки прогона. <c>null</c> — значения по умолчанию.
    /// </param>
    /// <param name="progress">
    /// Необязательный канал наблюдения: получает снимок каждого завершённого
    /// поколения. Отделён от <see cref="ITermination{TChromosome}"/> намеренно:
    /// критерий останова решает, когда остановиться, а не как показывать прогресс.
    /// <c>null</c> — наблюдение не нужно.
    /// </param>
    /// <param name="cancellationToken">Признак запрошенной отмены.</param>
    IGeneticAlgorithmResult<TChromosome> Run(
        IGeneticProblem<TChromosome> problem,
        GeneticAlgorithmOptions? options = null,
        IProgress<IGenerationResult>? progress = null,
        CancellationToken cancellationToken = default);
}
