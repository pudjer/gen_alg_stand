namespace Core.Contracts;

public interface IObserver<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    void OnGeneration(IReadOnlyList<Individ<TChromosome>> population);

    public IDisposable Subscribe(Action<IReadOnlyList<Individ<TChromosome>>> handler);

}