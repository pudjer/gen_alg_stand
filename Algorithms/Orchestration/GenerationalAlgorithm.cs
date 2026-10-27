using Core.Contracts;

namespace Algorithms.Orchestration;

/// <summary>
/// A generational genetic algorithm assembled purely from the operators of a
/// <see cref="GeneticProblem{TChromosome}"/>: selection, crossover, mutation and loss.
/// </summary>
/// <remarks>
/// The algorithm owns no behaviour of its own beyond scheduling. Elitism is derived from the
/// problem's <see cref="GeneticProblem{TChromosome}.LossFunction"/>, so every component is taken
/// from the problem instance supplied by the caller.
/// </remarks>
/// <typeparam name="TChromosome">The chromosome type operated on by the algorithm.</typeparam>
public sealed class GenerationalAlgorithm<TChromosome> : IAlgorithm<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    private readonly GeneticProblem<TChromosome> _problem;
    private readonly GenerationalOptions _options;
    private int _generation;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenerationalAlgorithm{TChromosome}"/> class.
    /// </summary>
    /// <param name="problem">The problem whose four operators drive every generation.</param>
    /// <param name="options">Scheduling options; defaults to <see cref="GenerationalOptions.Default"/> when <see langword="null"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="options"/> contains a negative generation limit or elite count.
    /// </exception>
    public GenerationalAlgorithm(GeneticProblem<TChromosome> problem, GenerationalOptions? options = null)
    {
        GenerationalOptions resolved = options ?? GenerationalOptions.Default;

        if (resolved.MaxGenerations < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "MaxGenerations must be non-negative.");
        }

        if (resolved.EliteCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "EliteCount must be non-negative.");
        }

        _problem = problem;
        _options = resolved;
        _generation = 0;
    }

    /// <summary>
    /// Produces the next generation by running selection, crossover, mutation and elitism.
    /// </summary>
    /// <param name="population">The current population to evolve.</param>
    /// <param name="random">The source of randomness passed to the problem's operators.</param>
    /// <returns>
    /// The next generation, or <see langword="null"/> once the generation limit is reached or the
    /// population is empty.
    /// </returns>
    public IReadOnlyList<TChromosome>? Iterate(IReadOnlyList<TChromosome> population, IRandom random)
    {
        ArgumentNullException.ThrowIfNull(population);
        ArgumentNullException.ThrowIfNull(random);

        if (_generation >= _options.MaxGenerations || population.Count == 0)
        {
            return null;
        }

        IReadOnlyList<TChromosome> selected = _problem.Selection.Select(population, random);
        IReadOnlyList<TChromosome> children = _problem.Crossover.Crossover(selected, random);
        IReadOnlyList<TChromosome> mutated = _problem.Mutation.Mutate(children, random);
        IReadOnlyList<TChromosome> next = ApplyElitism(population, mutated);

        if (_options.MaintainPopulationSize && next.Count != population.Count)
        {
            next = Resize(next, population.Count, random);
        }

        _generation++;
        return next;
    }

    /// <summary>
    /// Copies the best parents, ranked by the problem's loss function, over the worst offspring.
    /// </summary>
    /// <remarks>Chromosomes are immutable, so elites are reused by reference.</remarks>
    private IReadOnlyList<TChromosome> ApplyElitism(
        IReadOnlyList<TChromosome> parents,
        IReadOnlyList<TChromosome> offspring)
    {
        int eliteCount = Math.Min(_options.EliteCount, Math.Min(parents.Count, offspring.Count));
        if (eliteCount <= 0 || offspring.Count == 0)
        {
            return offspring;
        }

        TChromosome[] elites = Best(parents, eliteCount, ascending: true);
        TChromosome[] result = [.. offspring];
        int[] worst = WorstIndexes(result, eliteCount);

        for (int i = 0; i < eliteCount; i++)
        {
            result[worst[i]] = elites[i];
        }

        return result;
    }

    private TChromosome[] Best(IReadOnlyList<TChromosome> population, int count, bool ascending)
    {
        TChromosome[] sorted = [.. population];
        double[] losses = new double[sorted.Length];
        for (int i = 0; i < sorted.Length; i++)
        {
            losses[i] = _problem.LossFunction.Evaluate(sorted[i]);
        }

        Array.Sort(losses, sorted);

        if (!ascending)
        {
            Array.Reverse(sorted);
        }

        if (count >= sorted.Length)
        {
            return sorted;
        }

        TChromosome[] best = new TChromosome[count];
        Array.Copy(sorted, best, count);
        return best;
    }

    private int[] WorstIndexes(IReadOnlyList<TChromosome> population, int count)
    {
        int[] indexes = new int[population.Count];
        double[] losses = new double[population.Count];

        for (int i = 0; i < population.Count; i++)
        {
            indexes[i] = i;
            losses[i] = _problem.LossFunction.Evaluate(population[i]);
        }

        Array.Sort(losses, indexes);
        Array.Reverse(indexes);

        int[] worst = new int[count];
        Array.Copy(indexes, worst, count);
        return worst;
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
