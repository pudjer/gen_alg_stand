namespace Core.Contracts;

public interface IMutation<TChromosome>
    where TChromosome : IChromosome
{
    IReadOnlyList<TChromosome> Mutate(IReadOnlyList<TChromosome> population, IRandom random);
}
