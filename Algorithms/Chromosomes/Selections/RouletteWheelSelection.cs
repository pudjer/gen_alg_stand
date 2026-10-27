using Core.Contracts;

namespace Algorithms.Selections;

/// <summary>
/// Selects individuals by fitness-proportionate sampling, turning losses into weights by linear
/// inversion within the current population.
/// </summary>
/// <remarks>
/// The loss function is trusted only for its contract: lower is better. No assumption is made about
/// the sign or scale of the values, so this works for losses that are negative (for example the
/// negated OneMax count), unbounded, or arbitrary. Weights are derived relative to the population's
/// own worst and best loss, which keeps the selection pressure independent of the loss scale.
/// </remarks>
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

        List<TChromosome> result = new(count);

        double[] losses = new double[count];
        for (int i = 0; i < count; i++)
        {
            losses[i] = _lossFunction.Evaluate(population[i]);
        }

        double[] weights = ComputeWeights(losses);
        double total = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            total += weights[i];
        }

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

    /// <summary>
    /// Converts losses into positive weights by linear inversion relative to the population's span.
    /// </summary>
    /// <remarks>
    /// An offset of <c>span / count</c> keeps the worst individual selectable, so no chromosome is
    /// excluded outright. When the span is zero (all losses equal, or non-finite values present) the
    /// weights are uniform, which degrades gracefully to random selection.
    /// </remarks>
    private static double[] ComputeWeights(double[] losses)
    {
        int count = losses.Length;
        double[] weights = new double[count];

        double min = double.PositiveInfinity;
        double max = double.NegativeInfinity;

        for (int i = 0; i < count; i++)
        {
            double loss = losses[i];
            if (!double.IsFinite(loss))
            {
                return Uniform(count);
            }

            if (loss < min)
            {
                min = loss;
            }

            if (loss > max)
            {
                max = loss;
            }
        }

        double span = max - min;
        if (!(span > 0))
        {
            return Uniform(count);
        }

        double offset = span / count;
        for (int i = 0; i < count; i++)
        {
            weights[i] = (max - losses[i]) + offset;
        }

        return weights;
    }

    private static double[] Uniform(int count)
    {
        double[] weights = new double[count];
        for (int i = 0; i < count; i++)
        {
            weights[i] = 1.0;
        }

        return weights;
    }
}
