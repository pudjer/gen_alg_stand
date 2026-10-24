namespace Core.Contracts.Outer;

/// <summary>
/// Готовая реализация <see cref="IGeneticProblem{TChromosome}"/> — тупой носитель
/// компонентов задачи. Избавляет от ручного класса на шесть свойств.
/// <para>
/// Поля задаются через инициализаторы, а <c>with</c> позволяет получить вариант
/// задачи, заменив один компонент: например, другую селекцию при той же
/// целевой функции.
/// </para>
/// <para>
/// Интерфейс остаётся ради собственных реализаций: если задаче нужна
/// ленивая инициализация компонентов или зависимость от DI-контейнера,
/// реализуйте <see cref="IGeneticProblem{TChromosome}"/> напрямую.
/// </para>
/// </summary>
/// <typeparam name="TChromosome">Конкретный тип хромосомы задачи.</typeparam>
public sealed record GeneticProblem<TChromosome> : IGeneticProblem<TChromosome>
    where TChromosome : IChromosome
{
    public required ILossFunction<TChromosome> LossFunction { get; init; }

    public required IInitialPopulation<TChromosome> InitialPopulation { get; init; }

    public required ISelection<TChromosome> Selection { get; init; }

    public required ICrossover<TChromosome> Crossover { get; init; }

    public required IMutation<TChromosome> Mutation { get; init; }

    public required ITermination<TChromosome> Termination { get; init; }
}
