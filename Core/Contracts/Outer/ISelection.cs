using Core.Contracts.Inner;

namespace Core.Contracts.Outer;

/// <summary>
/// Стратегия отбора родителей.
/// </summary>
public interface ISelection
{
    /// <summary>
    /// Возвращает родителей для следующего поколения. Сколько их будет —
    /// решает сама стратегия: движок не знает размера популяции.
    /// Направление оптимизации доступно через
    /// <see cref="IExecutionContext.IsMaximization"/>.
    /// </summary>
    IReadOnlyList<Individual> Select(IExecutionContext context);
}
