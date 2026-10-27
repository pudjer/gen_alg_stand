using Core.Contracts;

namespace Algorithms.Orchestration;

/// <summary>
/// Scheduling knobs for <see cref="IslandModelAlgorithm{TChromosome}"/>.
/// </summary>
/// <param name="IslandCount">How many independent sub-populations to evolve in parallel.</param>
/// <param name="MaxGenerations">The maximum number of generations each island evolves.</param>
/// <param name="MigrationInterval">
/// How often, in generations, migrants are exchanged between neighbouring islands.
/// A value of <c>0</c> or less disables migration.
/// </param>
/// <param name="MigrantsPerIsland">How many of the best chromosomes each island sends to its neighbour.</param>
/// <param name="EliteCount">How many elites each island keeps per generation.</param>
/// <param name="RandomFactory">
/// Creates an independent <see cref="IRandom"/> per island. When <see langword="null"/>, seeds are
/// drawn sequentially from the algorithm's own random source, which keeps runs reproducible.
/// </param>
/// <param name="MaxDegreeOfParallelism">
/// The upper bound on concurrently evolving islands; <c>-1</c> means no limit.
/// </param>
public sealed record IslandOptions(
    int IslandCount = 4,
    int MaxGenerations = 250,
    int MigrationInterval = 10,
    int MigrantsPerIsland = 1,
    int EliteCount = 1,
    Func<int, IRandom>? RandomFactory = null,
    int MaxDegreeOfParallelism = -1)
{
    /// <summary>Gets a shared default configuration instance.</summary>
    public static IslandOptions Default { get; } = new();

    /// <summary>
    /// Gets a value indicating whether the islands are allowed to run concurrently.
    /// </summary>
    internal bool CanRunInParallel => IslandCount > 1;

    /// <summary>
    /// Gets the effective degree of parallelism to hand to <see cref="System.Threading.Tasks.Parallel"/>.
    /// </summary>
    internal int ResolvedDegreeOfParallelism => MaxDegreeOfParallelism <= 0
        ? Environment.ProcessorCount
        : Math.Min(MaxDegreeOfParallelism, Environment.ProcessorCount);
}
