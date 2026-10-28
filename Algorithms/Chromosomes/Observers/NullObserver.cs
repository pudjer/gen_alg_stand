using Core.Contracts;

namespace Algorithms.Observers;

public sealed class NullObserver<TChromosome> : Core.Contracts.IObserver<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    private static readonly NullObserver<TChromosome> SharedInstance = new();

    private NullObserver()
    {
    }

    public static NullObserver<TChromosome> Instance => SharedInstance;

    public void OnGeneration(IReadOnlyList<TChromosome> population)
    {
        ArgumentNullException.ThrowIfNull(population);
    }
}
