using Core.Contracts;

namespace Algorithms.Factories;

/// <summary>
/// An <see cref="IAlgorithmFactory{TChromosome}"/> that delegates algorithm creation to a delegate,
/// letting callers plug any algorithm without adding a dedicated factory type.
/// </summary>
/// <typeparam name="TChromosome">The chromosome type produced by the created algorithms.</typeparam>
public sealed class DelegatingAlgorithmFactory<TChromosome> : IAlgorithmFactory<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    private readonly Func<GeneticProblem<TChromosome>, IAlgorithm<TChromosome>> _factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="DelegatingAlgorithmFactory{TChromosome}"/> class.
    /// </summary>
    /// <param name="factory">The function that builds an algorithm for a given problem.</param>
    /// <exception cref="ArgumentNullException"><paramref name="factory"/> is <see langword="null"/>.</exception>
    public DelegatingAlgorithmFactory(Func<GeneticProblem<TChromosome>, IAlgorithm<TChromosome>> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        _factory = factory;
    }

    /// <summary>Creates an algorithm by invoking the supplied delegate.</summary>
    /// <param name="problem">The problem the algorithm will solve.</param>
    /// <returns>The algorithm produced by the delegate.</returns>
    public IAlgorithm<TChromosome> Create(GeneticProblem<TChromosome> problem) => _factory(problem);
}
