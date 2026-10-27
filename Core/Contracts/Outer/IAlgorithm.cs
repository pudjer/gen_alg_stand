namespace Core.Contracts;

public interface IAlgorithm<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    void Run(GeneticProblem<TChromosome> problem);
}