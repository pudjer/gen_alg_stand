namespace Core.Contracts;

public interface IAlgorithmFactory<TChromosome>
    where TChromosome : IChromosome
{
    IAlgorithm<TChromosome> Create(GeneticProblem<TChromosome> problem);
}