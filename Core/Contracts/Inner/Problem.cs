namespace Core.Contracts;

public struct GeneticProblem<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    public required ILossFunction<TChromosome> LossFunction { get; init; }
    public required ISelection<TChromosome> Selection { get; init; }
    public required ICrossover<TChromosome> Crossover { get; init; }
    public required IMutation<TChromosome> Mutation { get; init; }
    public required IInitialPopulation<TChromosome> InitialPopulation { get; init; }
    public required IObserver<TChromosome> Observer { get; init; }
    public required IRandomFactory RandomFactory { get; init; }
}