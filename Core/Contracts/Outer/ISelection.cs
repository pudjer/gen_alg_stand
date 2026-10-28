namespace Core.Contracts;

public interface ISelection<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    IReadOnlyList<Individ<TChromosome>> Select(IReadOnlyList<Individ<TChromosome>> population, IRandom random);
}
