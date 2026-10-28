namespace Algorithms.Orchestration;

public sealed record GenerationalOptions(
    int MaxGenerations = 1000,
    int EliteCount = 1,
    bool MaintainPopulationSize = true)
{
    public static GenerationalOptions Default { get; } = new();
}
