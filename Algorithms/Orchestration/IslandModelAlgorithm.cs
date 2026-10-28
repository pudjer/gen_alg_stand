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
        IReadOnlyList<Individ<TChromosome>>[] islands = new IReadOnlyList<Individ<TChromosome>>[islandCount];

        for (int i = 0; i < islandCount; i++)
        {
            IReadOnlyList<TChromosome> initial = problem.InitialPopulation.Create(randoms[i])
                ?? throw new InvalidOperationException("The initial population must not be null.");

            islands[i] = GenerationalAlgorithm<TChromosome>.Wrap(initial, problem.LossFunction);
        }

        GenerationalAlgorithm<TChromosome>[] steps = new GenerationalAlgorithm<TChromosome>[islandCount];
        for (int i = 0; i < islandCount; i++)
        {
            steps[i] = new GenerationalAlgorithm<TChromosome>(_islandOptions);
        }

        Func<int, IReadOnlyList<Individ<TChromosome>>?> step = index =>
            steps[index].Step(problem, islands[index], randoms[index]);

        problem.Observer.OnGeneration(Merge(islands));

        for (int generation = 0; generation < _options.MaxGenerations; generation++)
        {
            EvolveGeneration(islands, step);

            if (ShouldMigrate(generation))
            {
                Migrate(islands);
            }

            problem.Observer.OnGeneration(Merge(islands));
        }
    }

    private void EvolveGeneration(
        IReadOnlyList<Individ<TChromosome>>[] islands,
        Func<int, IReadOnlyList<Individ<TChromosome>>?> step)
    {
        void Advance(int index)
        {
            IReadOnlyList<Individ<TChromosome>>? next = step(index);
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

    private void Migrate(IReadOnlyList<Individ<TChromosome>>[] islands)
    {
        int count = islands.Length;
        if (count < 2 || _options.MigrantsPerIsland <= 0)
        {
            return;
        }

        Individ<TChromosome>[][] outgoing = new Individ<TChromosome>[count][];
        for (int i = 0; i < count; i++)
        {
            outgoing[i] = TopBest(islands[i], _options.MigrantsPerIsland);
        }

        for (int i = 0; i < count; i++)
        {
            int target = (i + 1) % count;
            islands[target] = Inject(islands[target], outgoing[i]);
        }
    }

    private static IReadOnlyList<Individ<TChromosome>> Inject(
        IReadOnlyList<Individ<TChromosome>> population,
        IReadOnlyList<Individ<TChromosome>> migrants)
    {
        if (population.Count == 0 || migrants.Count == 0)
        {
            return population;
        }

        int count = Math.Min(migrants.Count, population.Count);
        Individ<TChromosome>[] result = [.. population];
        int[] worst = PopulationRanking.WorstIndexes(result, count);

        for (int i = 0; i < count; i++)
        {
            result[worst[i]] = migrants[i];
        }

        return result;
    }

    private static Individ<TChromosome>[] TopBest(
        IReadOnlyList<Individ<TChromosome>> population,
        int count) =>
        PopulationRanking.Best(population, count);

    private static IReadOnlyList<Individ<TChromosome>> Merge(IReadOnlyList<Individ<TChromosome>>[] islands)
    {
        int total = 0;
        for (int i = 0; i < islands.Length; i++)
        {
            total += islands[i].Count;
        }

        Individ<TChromosome>[] merged = new Individ<TChromosome>[total];
        int offset = 0;

        for (int i = 0; i < islands.Length; i++)
        {
            IReadOnlyList<Individ<TChromosome>> island = islands[i];
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
