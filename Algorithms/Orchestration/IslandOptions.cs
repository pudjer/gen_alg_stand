using Core.Contracts;

namespace Algorithms.Orchestration;

/// <summary>
/// Controls how <see cref="IslandModelAlgorithm{TChromosome}"/> splits and connects sub-populations.
/// </summary>
public sealed record IslandOptions
{
    /// <summary>Gets the default options: four islands migrating one individual every ten steps.</summary>
    public static IslandOptions Default { get; } = new();

    /// <summary>Gets the number of islands the initial population is split into.</summary>
    public int IslandCount { get; init; } = 4;

    /// <summary>Gets the number of steps between two migrations. Zero disables migration.</summary>
    public int MigrationInterval { get; init; } = 10;

    /// <summary>Gets the number of best individuals sent from each island to its neighbour.</summary>
    public int MigrantsPerIsland { get; init; } = 1;

    /// <summary>
    /// Gets an optional factory that creates one independent random source per island from a seed
    /// drawn from the algorithm's shared <see cref="IRandom"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When this is <see langword="null"/> the islands run sequentially, sharing the single
    /// <see cref="IRandom"/> supplied to <see cref="IAlgorithm{TChromosome}.Iterate"/>.
    /// </para>
    /// <para>
    /// When it is set and <see cref="IslandCount"/> is greater than one, every island evolves in
    /// its own worker on the thread pool, using its own random source. Seeds are drawn one at a
    /// time before the workers start, so a given seeded <see cref="IRandom"/> still produces a
    /// reproducible run.
    /// </para>
    /// <para>
    /// Enabling this requires the problem's operators
    /// (<see cref="GeneticProblem{TChromosome}.Selection"/>,
    /// <see cref="GeneticProblem{TChromosome}.Crossover"/>,
    /// <see cref="GeneticProblem{TChromosome}.Mutation"/> and
    /// <see cref="GeneticProblem{TChromosome}.LossFunction"/>) to be stateless, or at least safe
    /// for concurrent calls. All operators shipped in this project satisfy that.
    /// </para>
    /// </remarks>
    public Func<int, IRandom>? RandomFactory { get; init; }

    /// <summary>
    /// Gets the maximum number of islands evolved concurrently. Zero or one forces sequential
    /// execution even when <see cref="RandomFactory"/> is set; a negative value uses
    /// <see cref="Environment.ProcessorCount"/>.
    /// </summary>
    public int MaxDegreeOfParallelism { get; init; } = -1;

    /// <summary>Gets a value indicating whether islands may run on separate workers.</summary>
    internal bool CanRunInParallel =>
        RandomFactory is not null && IslandCount > 1 && MaxDegreeOfParallelism != 1;

    /// <summary>Resolves <see cref="MaxDegreeOfParallelism"/> to a concrete positive limit.</summary>
    internal int ResolvedDegreeOfParallelism =>
        MaxDegreeOfParallelism < 0 ? Environment.ProcessorCount : Math.Max(1, MaxDegreeOfParallelism);
}
