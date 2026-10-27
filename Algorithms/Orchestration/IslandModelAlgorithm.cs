using Core.Contracts;

namespace Algorithms.Orchestration;

/// <summary>
/// Evolves several independent sub-populations (islands) in parallel-later style, exchanging the
/// best individuals between neighbouring islands through the ring topology.
/// </summary>
/// <remarks>
/// Migration ranks individuals with <see cref="GeneticProblem{TChromosome}.LossFunction"/>, so the
/// island model reuses the same problem operators as the inner algorithms and adds no new
/// abstractions of its own.
/// </remarks>
/// <typeparam name="TChromosome">The chromosome type operated on by the algorithm.</typeparam>
public sealed class IslandModelAlgorithm<TChromosome> : IAlgorithm<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    private readonly GeneticProblem<TChromosome> _problem;
    private readonly Func<GeneticProblem<TChromosome>, IAlgorithm<TChromosome>> _islandFactory;
    private readonly IslandOptions _options;

    private List<List<TChromosome>>? _islands;
    private List<IAlgorithm<TChromosome>>? _algorithms;
    private bool[]? _finished;
    private IRandom[]? _islandRandoms;
    private int _steps;
    private bool _initialized;
    private bool _exhausted;

    /// <summary>
    /// Initializes a new instance of the <see cref="IslandModelAlgorithm{TChromosome}"/> class.
    /// </summary>
    /// <param name="problem">The problem whose operators are shared by every island.</param>
    /// <param name="islandFactory">Creates one inner algorithm per island from the same problem.</param>
    /// <param name="options">Island options; defaults to <see cref="IslandOptions.Default"/> when <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="islandFactory"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="options"/> contains invalid values.</exception>
    public IslandModelAlgorithm(
        GeneticProblem<TChromosome> problem,
        Func<GeneticProblem<TChromosome>, IAlgorithm<TChromosome>> islandFactory,
        IslandOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(islandFactory);

        IslandOptions resolved = options ?? IslandOptions.Default;

        if (resolved.IslandCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "IslandCount must be at least one.");
        }

        if (resolved.MigrationInterval < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "MigrationInterval must be non-negative.");
        }

        if (resolved.MigrantsPerIsland < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "MigrantsPerIsland must be at least one.");
        }

        _problem = problem;
        _islandFactory = islandFactory;
        _options = resolved;
    }

    /// <summary>
    /// Advances the model by one step: every unfinished island performs one generation, then
    /// migrants are exchanged when the migration interval elapses.
    /// </summary>
    /// <param name="population">The initial population; used only on the first call.</param>
    /// <param name="random">The source of randomness shared by the islands and migration.</param>
    /// <returns>
    /// The flattened population after the step, or <see langword="null"/> once every island has
    /// finished (the final population is still returned once before that).
    /// </returns>
    public IReadOnlyList<TChromosome>? Iterate(IReadOnlyList<TChromosome> population, IRandom random)
    {
        ArgumentNullException.ThrowIfNull(population);
        ArgumentNullException.ThrowIfNull(random);

        if (_exhausted)
        {
            return null;
        }

        if (!_initialized)
        {
            Initialize(population, random);

            if (_exhausted)
            {
                return null;
            }
        }

        List<List<TChromosome>> islands = _islands!;
        List<IAlgorithm<TChromosome>> algorithms = _algorithms!;
        bool[] finished = _finished!;

        bool allFinished = true;
        for (int i = 0; i < finished.Length; i++)
        {
            if (!finished[i])
            {
                allFinished = false;
                break;
            }
        }

        if (allFinished)
        {
            _exhausted = true;
            return null;
        }

        AdvanceIslands(islands, algorithms, finished, random);

        _steps++;

        if (_options.MigrationInterval > 0
            && islands.Count >= 2
            && _steps % _options.MigrationInterval == 0)
        {
            Migrate(islands);
        }

        List<TChromosome> flattened = new();
        for (int i = 0; i < islands.Count; i++)
        {
            flattened.AddRange(islands[i]);
        }

        return flattened;
    }

    private void Initialize(IReadOnlyList<TChromosome> population, IRandom random)
    {
        _initialized = true;

        if (population.Count == 0)
        {
            _exhausted = true;
            return;
        }

        TChromosome[] snapshot = [.. population];
        int islandCount = Math.Min(_options.IslandCount, snapshot.Length);
        int baseSize = snapshot.Length / islandCount;
        int remainder = snapshot.Length % islandCount;

        List<List<TChromosome>> islands = new(islandCount);
        List<IAlgorithm<TChromosome>> algorithms = new(islandCount);
        bool[] finished = new bool[islandCount];

        int index = 0;
        for (int i = 0; i < islandCount; i++)
        {
            int size = baseSize + (i < remainder ? 1 : 0);
            List<TChromosome> island = new(size);

            for (int j = 0; j < size; j++)
            {
                island.Add(snapshot[index++]);
            }

            islands.Add(island);
            algorithms.Add(_islandFactory(_problem));
        }

        _islands = islands;
        _algorithms = algorithms;
        _finished = finished;

        // Seeds are drawn one at a time from the shared source, so the sequence of seeds is
        // deterministic for a given seeded IRandom even though the workers run concurrently.
        _islandRandoms = new IRandom[islandCount];
        if (_options.CanRunInParallel)
        {
            Func<int, IRandom> factory = _options.RandomFactory!;
            for (int i = 0; i < islandCount; i++)
            {
                _islandRandoms[i] = factory(random.Next(int.MinValue, int.MaxValue));
            }
        }
        else
        {
            for (int i = 0; i < islandCount; i++)
            {
                _islandRandoms[i] = random;
            }
        }
    }

    /// <summary>
    /// Runs one generation on every unfinished island, either sequentially or on the thread pool.
    /// </summary>
    /// <remarks>
    /// Each island owns its slot in <see cref="_islands"/> and its own random source, so workers
    /// never share mutable state. Migration happens afterwards, on the calling thread.
    /// </remarks>
    private void AdvanceIslands(
        List<List<TChromosome>> islands,
        List<IAlgorithm<TChromosome>> algorithms,
        bool[] finished,
        IRandom random)
    {
        IRandom[] randoms = _islandRandoms ?? throw new InvalidOperationException("Islands are not initialized.");

        if (_options.CanRunInParallel)
        {
            ParallelOptions parallelOptions = new()
            {
                MaxDegreeOfParallelism = _options.ResolvedDegreeOfParallelism,
            };

            Parallel.For(0, islands.Count, parallelOptions, i =>
            {
                if (finished[i])
                {
                    return;
                }

                IReadOnlyList<TChromosome>? result = algorithms[i].Iterate(islands[i], randoms[i]);

                if (result is null)
                {
                    finished[i] = true;
                }
                else
                {
                    islands[i] = [.. result];
                }
            });

            return;
        }

        for (int i = 0; i < islands.Count; i++)
        {
            if (finished[i])
            {
                continue;
            }

            IReadOnlyList<TChromosome>? sequentialResult = algorithms[i].Iterate(islands[i], randoms[i]);

            if (sequentialResult is null)
            {
                finished[i] = true;
            }
            else
            {
                islands[i] = [.. sequentialResult];
            }
        }
    }

    /// <summary>
    /// Copies the best individuals of each island over the worst individuals of the next island,
    /// ranking both sides with the problem's loss function.
    /// </summary>
    /// <remarks>
    /// All emigrants are selected before any island is modified, so a replacement cannot cascade
    /// around the ring within a single migration.
    /// </remarks>
    private void Migrate(List<List<TChromosome>> islands)
    {
        int islandCount = islands.Count;
        List<TChromosome[]> emigrants = new(islandCount);

        for (int i = 0; i < islandCount; i++)
        {
            List<TChromosome> island = islands[i];
            int count = Math.Min(_options.MigrantsPerIsland, island.Count);

            TChromosome[] sorted = [.. island];
            double[] losses = new double[sorted.Length];
            for (int j = 0; j < sorted.Length; j++)
            {
                losses[j] = _problem.LossFunction.Evaluate(sorted[j]);
            }

            Array.Sort(losses, sorted);

            TChromosome[] best = new TChromosome[count];
            Array.Copy(sorted, best, count);
            emigrants.Add(best);
        }

        for (int i = 0; i < islandCount; i++)
        {
            List<TChromosome> target = islands[(i + 1) % islandCount];
            TChromosome[] incoming = emigrants[i];
            if (incoming.Length == 0)
            {
                continue;
            }

            double[] losses = new double[target.Count];
            for (int j = 0; j < target.Count; j++)
            {
                losses[j] = _problem.LossFunction.Evaluate(target[j]);
            }

            int[] indexes = new int[target.Count];
            for (int j = 0; j < indexes.Length; j++)
            {
                indexes[j] = j;
            }

            Array.Sort(losses, indexes);

            for (int m = 0; m < incoming.Length && m < indexes.Length; m++)
            {
                int slot = indexes[target.Count - 1 - m];
                target[slot] = incoming[m];
            }
        }
    }
}
