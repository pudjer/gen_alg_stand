namespace Core.Contracts;

public interface IInitialPopulation<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    IReadOnlyList<TChromosome> Create(IRandom random);
}
