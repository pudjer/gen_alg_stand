using Core.Contracts;

namespace Algorithms.Observers;

public sealed class CompositeObserver<TChromosome> : Core.Contracts.IObserver<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    private readonly Core.Contracts.IObserver<TChromosome>[] _observers;

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

    public void OnGeneration(IReadOnlyList<Individ<TChromosome>> population)
    {
        ArgumentNullException.ThrowIfNull(population);

        for (int i = 0; i < _observers.Length; i++)
        {
            _observers[i].OnGeneration(population);
        }
    }
}
