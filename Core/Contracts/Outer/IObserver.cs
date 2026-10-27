namespace Core.Contracts;

public interface IObserver<TChromosome>
    where TChromosome : IChromosome
{
    void OnGeneration(IReadOnlyList<TChromosome> population);
}