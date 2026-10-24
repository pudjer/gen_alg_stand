namespace Core.Contracts.Outer;

/// <summary>
/// Задача как набор подключаемых компонентов: только свойства, никакой логики.
/// Единственное, что меняется при добавлении новой целевой функции.
/// <para>
/// Outer-контракт: реализуется вне движка и подставляется в него.
/// </para>
/// </summary>
public interface IGeneticProblem
{
    IFitnessFunction FitnessFunction { get; }

    /// <summary>Откуда берётся первое поколение хромосом.</summary>
    IInitialPopulation InitialPopulation { get; }

    ISelection Selection { get; }

    ICrossover Crossover { get; }

    IMutation Mutation { get; }

    /// <summary>
    /// Критерий останова задачи. Лимит поколений и отмена
    /// подключаются движком независимо — дублировать их не нужно.
    /// </summary>
    ITermination Termination { get; }
}
