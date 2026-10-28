using Core.Contracts;

namespace Algorithms.Observers;

public sealed class BestChromosomeObserver<TChromosome> : Core.Contracts.IObserver<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    private readonly ILossFunction<TChromosome> _lossFunction;

    public BestChromosomeObserver(ILossFunction<TChromosome> lossFunction)
    {
        ArgumentNullException.ThrowIfNull(lossFunction);

        _lossFunction = lossFunction;
    }

    public TChromosome? Best { get; private set; }

    public double BestLoss { get; private set; } = double.PositiveInfinity;

    public int Generation { get; private set; }

    public void OnGeneration(IReadOnlyList<TChromosome> population)
    {
        ArgumentNullException.ThrowIfNull(population);

        Generation++;

        for (int i = 0; i < population.Count; i++)
        {
            TChromosome candidate = population[i];
            double loss = _lossFunction.Evaluate(candidate);

            if (loss < BestLoss)
            {
                BestLoss = loss;
                Best = candidate;
            }
        }
    }
}
