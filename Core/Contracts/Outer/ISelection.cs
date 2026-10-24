using Core.Contracts.Inner;

namespace Core.Contracts.Outer;

/// <summary>
/// Стратегия отбора родителей.
/// </summary>
/// <typeparam name="TChromosome">Конкретный тип хромосомы задачи.</typeparam>
public interface ISelection<TChromosome>
    where TChromosome : IChromosome
{
    /// <summary>
    /// Возвращает родителей для следующего поколения. Сколько их будет —
    /// решает сама стратегия: движок не знает размера популяции.
    /// Всё, что нужно для решения, передаётся аргументами: текущая
    /// популяция с оценками, направление оптимизации и случайность.
    /// </summary>
    IReadOnlyList<TChromosome> Select(
        IPopulation<TChromosome> population,
        bool maximize,
        IRandom random);
}
