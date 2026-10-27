namespace Algorithms.Orchestration;

/// <summary>
/// Controls how <see cref="GenerationalAlgorithm{TChromosome}"/> schedules the operators of a problem.
/// </summary>
public sealed record GenerationalOptions
{
    /// <summary>Gets the default options: 1000 generations, one elite, constant population size.</summary>
    public static GenerationalOptions Default { get; } = new();

    /// <summary>Gets the maximum number of generations produced before the algorithm stops.</summary>
    public int MaxGenerations { get; init; } = 1000;

    /// <summary>
    /// Gets the number of best parents copied unchanged into the next generation, evaluated with
    /// the problem's loss function. Zero disables elitism.
    /// </summary>
    public int EliteCount { get; init; } = 1;

    /// <summary>Gets a value indicating whether the next generation is resized to the current population size.</summary>
    public bool MaintainPopulationSize { get; init; } = true;
}
