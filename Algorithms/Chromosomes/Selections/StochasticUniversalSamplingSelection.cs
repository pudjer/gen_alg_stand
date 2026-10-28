using Core.Contracts;

namespace Algorithms.Selections;

public sealed class StochasticUniversalSamplingSelection<TChromosome> : ISelection<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    private readonly ILossFunction<TChromosome> _lossFunction;

    public StochasticUniversalSamplingSelection(ILossFunction<TChromosome> lossFunction)
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

        double[] losses = new double[count];
        for (int i = 0; i < count; i++)
        {
            losses[i] = _lossFunction.Evaluate(population[i]);
        }

        double[] weights = ComputeWeights(losses);
        double total = 0d;
        for (int i = 0; i < weights.Length; i++)
        {
            total += weights[i];
        }

        if (!double.IsFinite(total) || total <= 0d)
        {
            return SelectUniformly(population, random);
        }

        TChromosome[] result = new TChromosome[count];
        double pointer = random.NextDouble() * total;
        double spacing = total / count;
        int selected = 0;
        double cumulative = 0d;

        for (int i = 0; i < count; i++)
        {
            cumulative += weights[i];
            while (pointer < cumulative && selected < count)
            {
                result[selected++] = population[i];
                pointer += spacing;
            }
        }

        while (selected < count)
        {
            result[selected++] = population[count - 1];
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
        if (!(span > 0d))
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
            weights[i] = 1d;
        }

        return weights;
    }

    private static IReadOnlyList<TChromosome> SelectUniformly(
        IReadOnlyList<TChromosome> population,
        IRandom random)
    {
        TChromosome[] result = new TChromosome[population.Count];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = population[random.Next(0, population.Count)];
        }

        return result;
    }
}
