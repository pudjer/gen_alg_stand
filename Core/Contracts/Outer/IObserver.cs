namespace Core.Contracts;

public interface IObserver<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    void OnGeneration(IReadOnlyList<TChromosome> population);
}