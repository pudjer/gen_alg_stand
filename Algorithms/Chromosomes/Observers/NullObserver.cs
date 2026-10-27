using Core.Contracts;

namespace Algorithms.Observers;

/// <summary>
/// An observer that intentionally ignores every generation.
/// </summary>
/// <typeparam name="TChromosome">The chromosome type being observed.</typeparam>
public sealed class NullObserver<TChromosome> : Core.Contracts.IObserver<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    private static readonly NullObserver<TChromosome> SharedInstance = new();

    private NullObserver()
    {
    }

    /// <summary>
    /// Gets the shared, stateless instance of the observer.
    /// </summary>
    public static NullObserver<TChromosome> Instance => SharedInstance;

    /// <summary>
    /// Does nothing; the population is not inspected.
    /// </summary>
    /// <param name="population">The current population.</param>
    public void OnGeneration(IReadOnlyList<TChromosome> population)
    {
        ArgumentNullException.ThrowIfNull(population);
    }
}
