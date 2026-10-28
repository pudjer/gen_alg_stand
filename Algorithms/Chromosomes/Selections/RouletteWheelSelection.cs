using Core.Contracts;

namespace Algorithms.Selections;

public sealed class RouletteWheelSelection<TChromosome> : ISelection<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    private readonly ILossFunction<TChromosome> _lossFunction;

    public RouletteWheelSelection(ILossFunction<TChromosome> lossFunction)
    {
        ArgumentNullException.ThrowIfNull(lossFunction);
        _lossFunction = lossFunction;
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
