using Core.Contracts;

namespace Algorithms.Selections;

/// <summary>
/// Selects individuals using rank-based weights so that better ranks dominate regardless of loss scale.
/// </summary>
/// <typeparam name="TChromosome">The chromosome type being evolved.</typeparam>
public sealed class RankSelection<TChromosome> : ISelection<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    private readonly ILossFunction<TChromosome> _lossFunction;

    /// <summary>
    /// Initializes a new instance of the <see cref="RankSelection{TChromosome}"/> class.
    /// </summary>
    /// <param name="lossFunction">Fitness evaluator; lower values are better.</param>
    public RankSelection(ILossFunction<TChromosome> lossFunction)
    {
        ArgumentNullException.ThrowIfNull(lossFunction);
        _lossFunction = lossFunction;
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

        TChromosome[] sorted = [.. population];
        double[] losses = new double[count];
        for (int i = 0; i < count; i++)
        {
            losses[i] = _lossFunction.Evaluate(sorted[i]);
        }

        Array.Sort(losses, sorted);

        double[] weights = new double[count];
        double total = 0;
        for (int rank = 0; rank < count; rank++)
        {
            double weight = count - rank;
            weights[rank] = weight;
            total += weight;
        }

        List<TChromosome> result = new(count);

        if (!double.IsFinite(total) || total <= 0)
        {
            for (int i = 0; i < count; i++)
            {
                result.Add(sorted[random.Next(0, count)]);
            }

            return result;
        }

        for (int i = 0; i < count; i++)
        {
            double target = random.NextDouble() * total;
            double cumulative = 0;
            int chosen = count - 1;

            for (int j = 0; j < count; j++)
            {
                cumulative += weights[j];
                if (target < cumulative)
                {
                    chosen = j;
                    break;
                }
            }

            result.Add(sorted[chosen]);
        }

        return result;
    }
}
