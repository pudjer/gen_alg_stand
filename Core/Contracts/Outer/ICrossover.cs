namespace Core.Contracts;

public interface ICrossover<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    IReadOnlyList<TChromosome> Crossover(IReadOnlyList<Individ<TChromosome>> population, IRandom random);
}
