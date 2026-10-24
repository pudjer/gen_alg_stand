namespace Core.Contracts.Outer;

/// <summary>
/// Задача как набор подключаемых компонентов: только свойства, никакой логики.
/// Единственное, что меняется при добавлении новой целевой функции.
/// <para>
/// Outer-контракт: реализуется вне движка и подставляется в него.
/// Все компоненты связаны одним типом хромосомы — собрать задачу
/// из несогласованных частей не получится.
/// </para>
/// </summary>
/// <typeparam name="TChromosome">Конкретный тип хромосомы задачи.</typeparam>
public interface IGeneticProblem<TChromosome>
    where TChromosome : IChromosome
{
    IFitnessFunction<TChromosome> FitnessFunction { get; }

    /// <summary>Откуда берётся первое поколение хромосом.</summary>
    IInitialPopulation<TChromosome> InitialPopulation { get; }

    ISelection<TChromosome> Selection { get; }

    ICrossover<TChromosome> Crossover { get; }

    IMutation<TChromosome> Mutation { get; }

    ITermination<TChromosome> Termination { get; }

    /// <summary>
    /// Вероятность скрещивания пары родителей: <c>0…1</c>.
    /// Движок сам разыгрывает случайность — пользователь только задаёт порог.
    /// Если кроссовер не выполняется, оба родителя переходят в следующее
    /// поколение без изменений (кроме возможной мутации).
    /// </summary>
    double CrossoverProbability { get; }

    /// <summary>
    /// Вероятность мутации потомка: <c>0…1</c>.
    /// Движок сам разыгрывает случайность — пользователь только задаёт порог.
    /// При <c>0</c> мутации не происходит вовсе, при <c>1</c> — мутирует каждый
    /// потомок.
    /// </summary>
    double MutationProbability { get; }
}
