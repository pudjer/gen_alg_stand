namespace Core.Contracts;

public interface IAlgorithm<TChromosome>
    where TChromosome : IChromosome
{
    IReadOnlyList<TChromosome>? Iterate(IReadOnlyList<TChromosome> population, IRandom random);
}