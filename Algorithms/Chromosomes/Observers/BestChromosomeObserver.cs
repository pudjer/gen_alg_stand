using Core.Contracts;

namespace Algorithms.Observers;

public sealed class BestChromosomeObserver<TChromosome> : Core.Contracts.IObserver<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    public BestChromosomeObserver()
    {
    }

    public Individ<TChromosome>? Best { get; private set; }

    public double BestLoss { get; private set; } = double.PositiveInfinity;

    public int Generation { get; private set; }

    public void OnGeneration(IReadOnlyList<Individ<TChromosome>> population)
    {
        ArgumentNullException.ThrowIfNull(population);

        Generation++;

        for (int i = 0; i < population.Count; i++)
        {
            Individ<TChromosome> candidate = population[i];
            double loss = candidate.LossValue;

            if (loss < BestLoss)
            {
                BestLoss = loss;
                Best = candidate;
            }
        }
    }
}
