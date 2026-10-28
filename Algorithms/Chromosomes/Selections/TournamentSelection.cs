using Core.Contracts;

namespace Algorithms.Selections;

public sealed class TournamentSelection<TChromosome> : ISelection<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    private readonly ILossFunction<TChromosome> _lossFunction;
    private readonly int _tournamentSize;

    public TournamentSelection(ILossFunction<TChromosome> lossFunction, int tournamentSize = 3)
    {
        ArgumentNullException.ThrowIfNull(lossFunction);
        ArgumentOutOfRangeException.ThrowIfLessThan(tournamentSize, 1, nameof(tournamentSize));

        _lossFunction = lossFunction;
        _tournamentSize = tournamentSize;
    }

    public IReadOnlyList<TChromosome> Select(IReadOnlyList<TChromosome> population, IRandom random)
    {
        ArgumentNullException.ThrowIfNull(population);
        ArgumentNullException.ThrowIfNull(random);

        int count = population.Count;
        if (count == 0)
        {
            return Array.Empty<TChromosome>();
        }

        double[] losses = new double[count];
        for (int i = 0; i < count; i++)
        {
            losses[i] = _lossFunction.Evaluate(population[i]);
        }

        List<TChromosome> result = new(count);
        for (int i = 0; i < count; i++)
        {
            int bestIndex = random.Next(0, count);
            double bestLoss = losses[bestIndex];

            for (int contestant = 1; contestant < _tournamentSize; contestant++)
            {
                int candidateIndex = random.Next(0, count);
                double candidateLoss = losses[candidateIndex];
                if (candidateLoss < bestLoss)
                {
                    bestIndex = candidateIndex;
                    bestLoss = candidateLoss;
                }
            }

            result.Add(population[bestIndex]);
        }

        return result;
    }
}
