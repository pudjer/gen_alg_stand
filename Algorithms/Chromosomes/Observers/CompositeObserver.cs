using Core.Contracts;

namespace Algorithms.Observers;

/// <summary>
/// An observer that forwards each generation to a fixed set of inner observers, in order.
/// </summary>
/// <typeparam name="TChromosome">The chromosome type being observed.</typeparam>
public sealed class CompositeObserver<TChromosome> : Core.Contracts.IObserver<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    private readonly Core.Contracts.IObserver<TChromosome>[] _observers;

    /// <summary>
    /// Creates a composite observer over the supplied observers.
    /// </summary>
    /// <param name="observers">The observers to notify, in order.</param>
    /// <exception cref="ArgumentNullException">The array or one of its elements is <see langword="null"/>.</exception>
    public CompositeObserver(params Core.Contracts.IObserver<TChromosome>[] observers)
    {
        ArgumentNullException.ThrowIfNull(observers);

        for (int i = 0; i < observers.Length; i++)
        {
            if (observers[i] is null)
            {
                throw new ArgumentNullException(nameof(observers), $"Observer at index {i} is null.");
            }
        }

        _observers = (Core.Contracts.IObserver<TChromosome>[])observers.Clone();
    }

    /// <summary>
    /// Forwards the population to every inner observer in registration order.
    /// </summary>
    /// <param name="population">The current population.</param>
    public void OnGeneration(IReadOnlyList<TChromosome> population)
    {
        ArgumentNullException.ThrowIfNull(population);

        for (int i = 0; i < _observers.Length; i++)
        {
            _observers[i].OnGeneration(population);
        }
    }
}
