namespace Core.Contracts;


public struct Input<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    public required GeneticProblem<TChromosome> Problem { get; init; }
    public required IInitialPopulation<TChromosome> InitialPopulation { get; init; }
    public required IObserver<TChromosome> Observer { get; init; }
    public required IAlgorithmFactory<TChromosome> AlgorithmFactory { get; init; }
}