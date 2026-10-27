namespace Core.Contracts;

public interface ICrossover<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    IReadOnlyList<TChromosome> Crossover(IReadOnlyList<TChromosome> population, IRandom random);
}
