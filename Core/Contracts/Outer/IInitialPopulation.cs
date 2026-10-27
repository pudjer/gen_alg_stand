namespace Core.Contracts;

public interface IInitialPopulation<TChromosome>
    where TChromosome : IChromosome
{
    IReadOnlyList<TChromosome> Create(IRandom random);
}
