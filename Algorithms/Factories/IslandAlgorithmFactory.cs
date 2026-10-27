using Algorithms.Orchestration;
using Core.Contracts;

namespace Algorithms.Factories;

/// <summary>
/// Builds an <see cref="IslandModelAlgorithm{TChromosome}"/> and hands the very same problem to
/// every island.
/// </summary>
/// <typeparam name="TChromosome">The chromosome type handled by the created algorithms.</typeparam>
public sealed class IslandAlgorithmFactory<TChromosome> : IAlgorithmFactory<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    private readonly IslandOptions _options;
    private readonly GenerationalOptions _islandGenerationalOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="IslandAlgorithmFactory{TChromosome}"/> class.
    /// </summary>
    /// <param name="options">Island options; defaults to <see cref="IslandOptions.Default"/> when <see langword="null"/>.</param>
    /// <param name="islandGenerationalOptions">
    /// Options for the generational algorithm run on each island; defaults to
    /// <see cref="GenerationalOptions.Default"/> when <see langword="null"/>.
    /// </param>
    public IslandAlgorithmFactory(
        IslandOptions? options = null,
        GenerationalOptions? islandGenerationalOptions = null)
    {
        _options = options ?? IslandOptions.Default;
        _islandGenerationalOptions = islandGenerationalOptions ?? GenerationalOptions.Default;
    }

    /// <summary>Creates an island model algorithm for the supplied problem.</summary>
    /// <param name="problem">The problem shared by all islands.</param>
    /// <returns>A new <see cref="IslandModelAlgorithm{TChromosome}"/>.</returns>
    public IAlgorithm<TChromosome> Create(GeneticProblem<TChromosome> problem) =>
        new IslandModelAlgorithm<TChromosome>(
            problem,
            p => new GenerationalAlgorithm<TChromosome>(p, _islandGenerationalOptions),
            _options);
}
