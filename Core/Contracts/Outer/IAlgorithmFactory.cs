namespace Core.Contracts;

public interface IAlgorithmFactory<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    IAlgorithm<TChromosome> Create(GeneticProblem<TChromosome> problem);
}