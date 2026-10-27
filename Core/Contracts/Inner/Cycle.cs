namespace Core.Contracts;
public sealed class GenAlgCycle<TChromosome>
    where TChromosome : IChromosome
{

    public required IRandom Random { get; init; }
    public void Run(
        Input<TChromosome> input,
        CancellationToken cancellationToken = default)
    {
        var algorithm = input.AlgorithmFactory.Create(input.Problem);
        IReadOnlyList<TChromosome> initPopulation = input.InitialPopulation.Create(Random);
        input.Observer.OnGeneration(initPopulation);

        IReadOnlyList<TChromosome>? population = initPopulation;
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            population = algorithm.Iterate(population, Random);
            if (population is null)
            {
                break;
            }
            input.Observer.OnGeneration(population);
        }
    }
}