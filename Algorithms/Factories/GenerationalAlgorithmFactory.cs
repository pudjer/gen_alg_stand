using Algorithms.Orchestration;
using Core.Contracts;

namespace Algorithms.Factories;

/// <summary>
/// Builds a <see cref="GenerationalAlgorithm{TChromosome}"/> from any <see cref="GeneticProblem{TChromosome}"/>.
/// </summary>
/// <remarks>The factory stores no problem state: all operators come from the problem it receives.</remarks>
/// <typeparam name="TChromosome">The chromosome type handled by the created algorithms.</typeparam>
public sealed class GenerationalAlgorithmFactory<TChromosome> : IAlgorithmFactory<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    private readonly GenerationalOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenerationalAlgorithmFactory{TChromosome}"/> class.
    /// </summary>
    /// <param name="options">Generation options; defaults to <see cref="GenerationalOptions.Default"/> when <see langword="null"/>.</param>
    public GenerationalAlgorithmFactory(GenerationalOptions? options = null)
    {
        _options = options ?? GenerationalOptions.Default;
    }

    /// <summary>Creates a generational algorithm for the supplied problem.</summary>
    /// <param name="problem">The problem whose operators drive the algorithm.</param>
    /// <returns>A new <see cref="GenerationalAlgorithm{TChromosome}"/>.</returns>
    public IAlgorithm<TChromosome> Create(GeneticProblem<TChromosome> problem) =>
        new GenerationalAlgorithm<TChromosome>(problem, _options);
}
