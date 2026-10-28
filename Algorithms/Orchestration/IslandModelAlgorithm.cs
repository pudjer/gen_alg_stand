using Core.Contracts;

namespace Algorithms.Orchestration;

public sealed class IslandModelAlgorithm<TChromosome> : IAlgorithm<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    private readonly IslandOptions _options;
    private readonly GenerationalOptions _islandOptions;

    public IslandModelAlgorithm(IslandOptions? options = null)
    {
        IslandOptions resolved = options ?? IslandOptions.Default;
        ArgumentOutOfRangeException.ThrowIfLessThan(resolved.IslandCount, 1, nameof(options));
        ArgumentOutOfRangeException.ThrowIfNegative(resolved.MaxGenerations, nameof(options));
        ArgumentOutOfRangeException.ThrowIfNegative(resolved.MigrantsPerIsland, nameof(options));
        ArgumentOutOfRangeException.ThrowIfNegative(resolved.EliteCount, nameof(options));

        _options = resolved;
        _islandOptions = new GenerationalOptions(
            MaxGenerations: 1,
            EliteCount: resolved.EliteCount,
            MaintainPopulationSize: resolved.MaintainPopulationSize);
    }

    public void Run(GeneticProblem<TChromosome> problem)
    {
        int islandCount = _options.IslandCount;
        IRandom[] randoms = CreateIslandRandoms(problem.RandomFactory, islandCount);
        IReadOnlyList<TChromosome>[] islands = new IReadOnlyList<TChromosome>[islandCount];

        for (int i = 0; i < islandCount; i++)
        {
            islands[i] = problem.InitialPopulation.Create(randoms[i])
                ?? throw new InvalidOperationException("The initial population must not be null.");
        }

        GenerationalAlgorithm<TChromosome>[] steps = new GenerationalAlgorithm<TChromosome>[islandCount];
        for (int i = 0; i < islandCount; i++)
        {
            steps[i] = new GenerationalAlgorithm<TChromosome>(_islandOptions);
        }

        Func<int, IReadOnlyList<TChromosome>?> step = index =>
            steps[index].Step(problem, islands[index], randoms[index]);

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

    private static IRandom[] CreateIslandRandoms(IRandomFactory randomFactory, int islandCount)
    {
        IRandom[] randoms = new IRandom[islandCount];

        for (int i = 0; i < islandCount; i++)
        {
            randoms[i] = randomFactory.Create()
                ?? throw new InvalidOperationException("The random factory returned null.");
        }

        return randoms;
    }
}
