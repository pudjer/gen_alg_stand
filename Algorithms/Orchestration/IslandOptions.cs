using Core.Contracts;

namespace Algorithms.Orchestration;

public sealed record IslandOptions(
    int IslandCount = 4,
    int MaxGenerations = 250,
    int MigrationInterval = 10,
    int MigrantsPerIsland = 1,
    int EliteCount = 1,
    bool MaintainPopulationSize = true,
    int MaxDegreeOfParallelism = -1)
{
    public static IslandOptions Default { get; } = new();

    internal bool CanRunInParallel => IslandCount > 1;

    internal int ResolvedDegreeOfParallelism => MaxDegreeOfParallelism <= 0
        ? Environment.ProcessorCount
        : Math.Min(MaxDegreeOfParallelism, Environment.ProcessorCount);
}
