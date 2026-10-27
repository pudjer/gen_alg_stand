namespace Core.Contracts;

public interface ISelection<TChromosome>
    where TChromosome : IChromosome
{
    IReadOnlyList<TChromosome> Select(IReadOnlyList<TChromosome> population, IRandom random);
}
