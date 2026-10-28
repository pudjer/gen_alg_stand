namespace Core.Contracts;

public sealed class SubscribableObserver<TChromosome> : IObserver<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    private readonly List<Action<IReadOnlyList<Individ<TChromosome>>>> _subscribers = [];

    public IDisposable Subscribe(Action<IReadOnlyList<Individ<TChromosome>>> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        lock (_subscribers)
        {
            _subscribers.Add(handler);
        }

        return new Unsubscriber(this, handler);
    }

    public void OnGeneration(IReadOnlyList<Individ<TChromosome>> population)
    {
        Action<IReadOnlyList<Individ<TChromosome>>>[] snapshot;

        lock (_subscribers)
        {
            snapshot = [.. _subscribers];
        }

        foreach (Action<IReadOnlyList<Individ<TChromosome>>> handler in snapshot)
        {
            handler(population);
        }
    }

    private sealed class Unsubscriber(
        SubscribableObserver<TChromosome> owner,
        Action<IReadOnlyList<Individ<TChromosome>>> handler) : IDisposable
    {
        public void Dispose()
        {
            lock (owner._subscribers)
            {
                owner._subscribers.Remove(handler);
            }
        }
    }
}
