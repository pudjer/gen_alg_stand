namespace Core.Contracts.Outer;

/// <summary>
/// Задача как набор подключаемых компонентов: только свойства, никакой логики.
/// Единственное, что меняется при добавлении новой целевой функции.
/// <para>
/// Outer-контракт: реализуется вне движка и подставляется в него.
/// Все компоненты связаны одним типом хромосомы — собрать задачу
/// из несогласованных частей не получится.
/// </para>
/// <para>
/// Здесь описывается только предметная область. Интенсивность операторов
/// и прочие настройки прогона — в <see cref="Core.Contracts.Inner.GeneticAlgorithmOptions"/>,
/// элитизм — политика движка (см. <see cref="Core.Contracts.Inner.IGeneticAlgorithm{TChromosome}"/>).
/// </para>
/// </summary>
/// <typeparam name="TChromosome">Конкретный тип хромосомы задачи.</typeparam>
public interface IGeneticProblem<TChromosome>
    where TChromosome : IChromosome
{
    ILossFunction<TChromosome> LossFunction { get; }

    /// <summary>Откуда берётся первое поколение хромосом.</summary>
    IInitialPopulation<TChromosome> InitialPopulation { get; }

    ISelection<TChromosome> Selection { get; }

    ICrossover<TChromosome> Crossover { get; }

    IMutation<TChromosome> Mutation { get; }

    ITermination<TChromosome> Termination { get; }
}
