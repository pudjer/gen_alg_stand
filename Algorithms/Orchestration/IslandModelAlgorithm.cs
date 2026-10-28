using Algorithms.Random;
using Core.Contracts;

namespace Algorithms.Orchestration;

/// <summary>
/// An island-model genetic algorithm: several independent sub-populations evolve in parallel and
/// periodically exchange their best chromosomes.
/// </summary>
/// <remarks>
/// <para>
/// Every island runs the same generational schedule as <see cref="GenerationalAlgorithm{TChromosome}"/>,
/// but owns a separate population and a separate <see cref="IRandom"/>. Because the island randomness
/// is derived deterministically from the algorithm's seed and islands only interact at generation
/// barriers, a run is reproducible for a fixed seed even though the islands execute concurrently.
/// </para>
/// <para>
/// Migration uses a ring topology: island <c>i</c> sends its best migrants to island <c>i + 1</c>,
/// replacing that island's worst chromosomes.
/// </para>
/// </remarks>
/// <typeparam name="TChromosome">The chromosome type operated on by the algorithm.</typeparam>
public sealed class IslandModelAlgorithm<TChromosome> : IAlgorithm<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    private readonly IRandom _random;
    private readonly IslandOptions _options;
    private readonly GenerationalOptions _islandOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="IslandModelAlgorithm{TChromosome}"/> class.
    /// </summary>
    /// <param name="random">
    /// The random source used to seed the islands. It is never shared with a worker, so it stays
    /// single-threaded.
    /// </param>
    /// <param name="options">Scheduling options; defaults to <see cref="IslandOptions.Default"/> when <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="random"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="options"/> contains invalid values.</exception>
    public IslandModelAlgorithm(IRandom random, IslandOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(random);

        IslandOptions resolved = options ?? IslandOptions.Default;
        ArgumentOutOfRangeException.ThrowIfLessThan(resolved.IslandCount, 1, nameof(options));
        ArgumentOutOfRangeException.ThrowIfNegative(resolved.MaxGenerations, nameof(options));
        ArgumentOutOfRangeException.ThrowIfNegative(resolved.MigrantsPerIsland, nameof(options));
        ArgumentOutOfRangeException.ThrowIfNegative(resolved.EliteCount, nameof(options));

        _random = random;
        _options = resolved;
        _islandOptions = new GenerationalOptions(
            MaxGenerations: 1,
            EliteCount: resolved.EliteCount,
            MaintainPopulationSize: resolved.MaintainPopulationSize);
    }

    /// <summary>
    /// Runs every island to completion and reports the merged population to the problem's observer.
    /// </summary>
    /// <param name="problem">The problem supplying all operators, the initial population and the observer.</param>
    public void Run(GeneticProblem<TChromosome> problem)
    {
        int islandCount = _options.IslandCount;
        IRandom[] randoms = CreateIslandRandoms(islandCount);
        IReadOnlyList<TChromosome>[] islands = new IReadOnlyList<TChromosome>[islandCount];

        for (int i = 0; i < islandCount; i++)
        {
            islands[i] = problem.InitialPopulation.Create(randoms[i])
                ?? throw new InvalidOperationException("The initial population must not be null.");
        }

        // One instance per island: the generational step is stateless, so the instances are only
        // recreated when the island configuration changes.
        GenerationalAlgorithm<TChromosome>[] steps = new GenerationalAlgorithm<TChromosome>[islandCount];
        for (int i = 0; i < islandCount; i++)
        {
            steps[i] = new GenerationalAlgorithm<TChromosome>(randoms[i], _islandOptions);
        }

        Func<int, IReadOnlyList<TChromosome>?> step = index =>
            steps[index].Step(problem, islands[index], randoms[index]);

        // Generation 0 is the initial population, exactly as in the generational algorithm, so the
        // island model produces the same number of observer callbacks for the same generation budget.
        problem.Observer.OnGeneration(Merge(islands));

        for (int generation = 0; generation < _options.MaxGenerations; generation++)
        {
            EvolveGeneration(islands, step);

            if (ShouldMigrate(generation))
            {
                Migrate(problem, islands);
            }

            problem.Observer.OnGeneration(Merge(islands));
        }
    }

    /// <summary>
    /// Advances every island by exactly one generation.
    /// </summary>
    private void EvolveGeneration(
        IReadOnlyList<TChromosome>[] islands,
        Func<int, IReadOnlyList<TChromosome>?> step)
    {
        void Advance(int index)
        {
            IReadOnlyList<TChromosome>? next = step(index);
            if (next is not null)
            {
                islands[index] = next;
            }
        }

        if (!_options.CanRunInParallel)
        {
            for (int i = 0; i < islands.Length; i++)
            {
                Advance(i);
            }

            return;
        }

        Parallel.For(
            0,
            islands.Length,
            new ParallelOptions { MaxDegreeOfParallelism = _options.ResolvedDegreeOfParallelism },
            Advance);
    }

    /// <summary>
    /// Exchanges the best chromosomes between neighbouring islands in a ring.
    /// </summary>
    /// <remarks>
    /// Migrants are collected from a snapshot of every island before any island is modified, so the
    /// exchange does not depend on iteration order.
    /// </remarks>
    private void Migrate(GeneticProblem<TChromosome> problem, IReadOnlyList<TChromosome>[] islands)
    {
        int count = islands.Length;
        if (count < 2 || _options.MigrantsPerIsland <= 0)
        {
            return;
        }

        TChromosome[][] outgoing = new TChromosome[count][];
        for (int i = 0; i < count; i++)
        {
            outgoing[i] = TopBest(problem, islands[i], _options.MigrantsPerIsland);
        }

        for (int i = 0; i < count; i++)
        {
            int target = (i + 1) % count;
            islands[target] = Inject(problem, islands[target], outgoing[i]);
        }
    }

    /// <summary>
    /// Replaces the worst chromosomes of <paramref name="population"/> with the supplied migrants.
    /// </summary>
    private static IReadOnlyList<TChromosome> Inject(
        GeneticProblem<TChromosome> problem,
        IReadOnlyList<TChromosome> population,
        IReadOnlyList<TChromosome> migrants)
    {
        if (population.Count == 0 || migrants.Count == 0)
        {
            return population;
        }

        int count = Math.Min(migrants.Count, population.Count);
        TChromosome[] result = [.. population];
        int[] worst = PopulationRanking.WorstIndexes(problem.LossFunction, result, count);

        for (int i = 0; i < count; i++)
        {
            result[worst[i]] = migrants[i];
        }

        return result;
    }

    private static TChromosome[] TopBest(
        GeneticProblem<TChromosome> problem,
        IReadOnlyList<TChromosome> population,
        int count) =>
        PopulationRanking.Best(problem.LossFunction, population, count);

    private static IReadOnlyList<TChromosome> Merge(IReadOnlyList<TChromosome>[] islands)
    {
        int total = 0;
        for (int i = 0; i < islands.Length; i++)
        {
            total += islands[i].Count;
        }

        TChromosome[] merged = new TChromosome[total];
        int offset = 0;

        for (int i = 0; i < islands.Length; i++)
        {
            IReadOnlyList<TChromosome> island = islands[i];
            for (int j = 0; j < island.Count; j++)
            {
                merged[offset++] = island[j];
            }
        }

        return merged;
    }

    private bool ShouldMigrate(int generation) =>
        _options.MigrationInterval > 0 && (generation + 1) % _options.MigrationInterval == 0;

    private IRandom[] CreateIslandRandoms(int islandCount)
    {
        IRandom[] randoms = new IRandom[islandCount];

        if (_options.RandomFactory is { } factory)
        {
            for (int i = 0; i < islandCount; i++)
            {
                randoms[i] = factory(i)
                    ?? throw new InvalidOperationException("The random factory returned null.");
            }

            return randoms;
        }

        for (int i = 0; i < islandCount; i++)
        {
            randoms[i] = new SeededRandom(_random.Next(int.MinValue, int.MaxValue));
        }

        return randoms;
    }
}
