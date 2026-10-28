using Core.Contracts;

namespace Setup;

public sealed class Setup<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    public required GeneticProblem<TChromosome> Problem { get; init; }

    public required IAlgorithm<TChromosome> Algorithm { get; init; }
}
