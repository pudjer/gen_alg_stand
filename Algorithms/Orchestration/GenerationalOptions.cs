namespace Algorithms.Orchestration;

/// <summary>
/// Scheduling knobs for <see cref="GenerationalAlgorithm{TChromosome}"/>.
/// </summary>
/// <param name="MaxGenerations">The maximum number of generations to evolve.</param>
/// <param name="EliteCount">How many of the best parents are copied into the next generation unchanged.</param>
/// <param name="MaintainPopulationSize">
/// When <see langword="true"/>, the next generation is resized back to the parent population size.
/// </param>
public sealed record GenerationalOptions(
    int MaxGenerations = 1000,
    int EliteCount = 1,
    bool MaintainPopulationSize = true)
{
    /// <summary>Gets a shared default configuration instance.</summary>
    public static GenerationalOptions Default { get; } = new();
}
