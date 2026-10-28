namespace Core.Contracts;

public interface IMutation<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    IReadOnlyList<TChromosome> Mutate(IReadOnlyList<Individ<TChromosome>> population, IRandom random);
}
