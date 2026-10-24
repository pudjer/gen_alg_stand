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
public interface IExecutionContext
{
    /// <summary>Номер текущего поколения, начиная с 0.</summary>
    int Generation { get; }

    /// <summary>Направление оптимизации целевой функции.</summary>
    bool IsMaximization { get; }

    /// <summary>Текущая популяция. При создании первого поколения пуста.</summary>
    IReadOnlyList<Individual> Population { get; }

    /// <summary>
    /// Лучшая особь за весь прогон. <c>null</c>, пока не оценена ни одна особь.
    /// </summary>
    Individual? Best { get; }

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
