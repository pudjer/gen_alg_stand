using Core.Contracts;

namespace Algorithms.Orchestration;

/// <summary>
/// A generational genetic algorithm that owns the whole evolution loop and drives it with the
/// operators carried by a <see cref="GeneticProblem{TChromosome}"/>.
/// </summary>
/// <remarks>
/// <para>
/// The algorithm only schedules work: it asks the problem for an initial population, then repeatedly
/// applies selection, crossover and mutation, reporting every generation to the problem's observer.
/// Elitism is derived from the problem's loss function.
/// </para>
/// <para>
/// Randomness is injected once through the constructor, so a run is fully determined by the seed of
/// that <see cref="IRandom"/>.
/// </para>
/// </remarks>
/// <typeparam name="TChromosome">The chromosome type operated on by the algorithm.</typeparam>
public sealed class GenerationalAlgorithm<TChromosome> : IAlgorithm<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    private readonly IRandom _random;
    private readonly GenerationalOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenerationalAlgorithm{TChromosome}"/> class.
    /// </summary>
    /// <param name="random">The random source shared by the problem's operators.</param>
    /// <param name="options">Scheduling options; defaults to <see cref="GenerationalOptions.Default"/> when <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="random"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="options"/> contains invalid values.</exception>
    public GenerationalAlgorithm(IRandom random, GenerationalOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(random);

        GenerationalOptions resolved = options ?? GenerationalOptions.Default;
        ArgumentOutOfRangeException.ThrowIfNegative(resolved.MaxGenerations, nameof(options));
        ArgumentOutOfRangeException.ThrowIfNegative(resolved.EliteCount, nameof(options));

        _random = random;
        _options = resolved;
    }

    /// <summary>
    /// Runs the full evolution: builds the initial population, then evolves it for at most
    /// <see cref="GenerationalOptions.MaxGenerations"/> generations.
    /// </summary>
    /// <param name="problem">The problem supplying all operators, the initial population and the observer.</param>
    public void Run(GeneticProblem<TChromosome> problem)
    {
        IReadOnlyList<TChromosome> population = problem.InitialPopulation.Create(_random)
            ?? throw new InvalidOperationException("The initial population must not be null.");

        problem.Observer.OnGeneration(population);

        for (int generation = 0; generation < _options.MaxGenerations; generation++)
        {
            IReadOnlyList<TChromosome>? next = Step(problem, population, _random);
            if (next is null)
            {
                return;
            }

            population = next;
            problem.Observer.OnGeneration(population);
        }
    }

    /// <summary>
    /// Produces one generation from the supplied population.
    /// </summary>
    /// <param name="problem">The problem supplying the operators.</param>
    /// <param name="population">The current population.</param>
    /// <param name="random">The random source to use for this step.</param>
    /// <returns>The next generation, or <see langword="null"/> when the population is empty.</returns>
    /// <remarks>
    /// This is the unit the island model interleaves; <see cref="Run"/> is a thin loop over it.
    /// </remarks>
    internal IReadOnlyList<TChromosome>? Step(
        GeneticProblem<TChromosome> problem,
        IReadOnlyList<TChromosome> population,
        IRandom random)
    {
        if (population.Count == 0)
        {
            return null;
        }

        IReadOnlyList<TChromosome> selected = problem.Selection.Select(population, random);
        IReadOnlyList<TChromosome> children = problem.Crossover.Crossover(selected, random);
        IReadOnlyList<TChromosome> mutated = problem.Mutation.Mutate(children, random);
        IReadOnlyList<TChromosome> next = ApplyElitism(problem, population, mutated);

        if (_options.MaintainPopulationSize && next.Count != population.Count)
        {
            next = Resize(next, population.Count, random);
        }

        return next;
    }

    /// <summary>
    /// Copies the best parents, ranked by the problem's loss function, over the worst offspring.
    /// </summary>
    /// <remarks>Chromosomes are immutable, so elites are reused by reference.</remarks>
    private IReadOnlyList<TChromosome> ApplyElitism(
        GeneticProblem<TChromosome> problem,
        IReadOnlyList<TChromosome> parents,
        IReadOnlyList<TChromosome> offspring)
    {
        int eliteCount = Math.Min(_options.EliteCount, Math.Min(parents.Count, offspring.Count));
        if (eliteCount <= 0)
        {
            return offspring;
        }

        TChromosome[] elites = PopulationRanking.Best(problem.LossFunction, parents, eliteCount);
        TChromosome[] result = [.. offspring];
        int[] worst = PopulationRanking.WorstIndexes(problem.LossFunction, result, eliteCount);

        for (int i = 0; i < eliteCount; i++)
        {
            result[worst[i]] = elites[i];
        }

        return result;
    }

    private static IReadOnlyList<TChromosome> Resize(
        IReadOnlyList<TChromosome> population,
        int targetCount,
        IRandom random)
    {
        if (population.Count >= targetCount)
        {
            TChromosome[] truncated = new TChromosome[targetCount];
            for (int i = 0; i < targetCount; i++)
            {
                truncated[i] = population[i];
            }

            return truncated;
        }

        List<TChromosome> padded = new(targetCount);
        for (int i = 0; i < population.Count; i++)
        {
            padded.Add(population[i]);
        }

        if (population.Count == 0)
        {
            return padded;
        }

        while (padded.Count < targetCount)
        {
            padded.Add(population[random.Next(0, population.Count)].Clone());
        }

        return padded;
    }
}
