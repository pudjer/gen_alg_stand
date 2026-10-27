namespace Core.Contracts;

public interface ICrossover<TChromosome>
    where TChromosome : IChromosome
{
    IReadOnlyList<TChromosome> Crossover(IReadOnlyList<TChromosome> population, IRandom random);
}
