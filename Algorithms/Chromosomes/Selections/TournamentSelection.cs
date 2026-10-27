using Core.Contracts;

namespace Algorithms.Selections;

/// <summary>
/// Selects individuals by running random tournaments and keeping the fittest contestant of each.
/// </summary>
/// <typeparam name="TChromosome">The chromosome type being evolved.</typeparam>
public sealed class TournamentSelection<TChromosome> : ISelection<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    private readonly ILossFunction<TChromosome> _lossFunction;
    private readonly int _tournamentSize;

    /// <summary>
    /// Initializes a new instance of the <see cref="TournamentSelection{TChromosome}"/> class.
    /// </summary>
    /// <param name="lossFunction">Fitness evaluator; lower values are better.</param>
    /// <param name="tournamentSize">Number of contestants per tournament; must be at least one.</param>
    public TournamentSelection(ILossFunction<TChromosome> lossFunction, int tournamentSize = 3)
    {
        ArgumentNullException.ThrowIfNull(lossFunction);
        ArgumentOutOfRangeException.ThrowIfLessThan(tournamentSize, 1, nameof(tournamentSize));

        _lossFunction = lossFunction;
        _tournamentSize = tournamentSize;
    }

    /// <inheritdoc />
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
