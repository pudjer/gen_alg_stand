using Core.Contracts;

namespace Algorithms.Selections;

/// <summary>
/// Selects individuals by fitness-proportionate sampling using inversely scaled loss weights.
/// </summary>
/// <typeparam name="TChromosome">The chromosome type being evolved.</typeparam>
public sealed class RouletteWheelSelection<TChromosome> : ISelection<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    private readonly ILossFunction<TChromosome> _lossFunction;

    /// <summary>
    /// Initializes a new instance of the <see cref="RouletteWheelSelection{TChromosome}"/> class.
    /// </summary>
    /// <param name="lossFunction">Fitness evaluator; lower values are better.</param>
    public RouletteWheelSelection(ILossFunction<TChromosome> lossFunction)
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

        double[] weights = new double[count];
        double total = 0;
        for (int i = 0; i < count; i++)
        {
            double loss = _lossFunction.Evaluate(population[i]);
            double weight = ComputeWeight(loss);
            weights[i] = weight;
            total += weight;
        }

        List<TChromosome> result = new(count);

        if (!double.IsFinite(total) || total <= 0)
        {
            for (int i = 0; i < count; i++)
            {
                result.Add(population[random.Next(0, count)]);
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

            result.Add(population[chosen]);
        }

        return result;
    }

    private static double ComputeWeight(double loss)
    {
        if (!double.IsFinite(loss) || loss < 0)
        {
            return 1.0;
        }

        return 1.0 / (1.0 + loss);
    }
}
