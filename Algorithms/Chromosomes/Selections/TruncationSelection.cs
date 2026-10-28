using Core.Contracts;

namespace Algorithms.Selections;

public sealed class TruncationSelection<TChromosome> : ISelection<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    private readonly double _survivalRate;

    public TruncationSelection(double survivalRate = 0.5)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(survivalRate, 0.0, nameof(survivalRate));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(survivalRate, 1.0, nameof(survivalRate));

        _survivalRate = survivalRate;
    }

    public IReadOnlyList<Individ<TChromosome>> Select(
        IReadOnlyList<Individ<TChromosome>> population,
        IRandom random)
    {
        ArgumentNullException.ThrowIfNull(population);
        ArgumentNullException.ThrowIfNull(random);

        int count = population.Count;
        if (count == 0)
        {
            return Array.Empty<Individ<TChromosome>>();
        }

        Individ<TChromosome>[] sorted = [.. population];
        double[] losses = new double[count];
        for (int i = 0; i < count; i++)
        {
            losses[i] = sorted[i].LossValue;
        }

        Array.Sort(losses, sorted);

        int survivorCount = Math.Max(1, (int)Math.Ceiling(count * _survivalRate));
        if (survivorCount > count)
        {
            survivorCount = count;
        }

        List<Individ<TChromosome>> result = new(count);
        for (int i = 0; i < count; i++)
        {
            result.Add(sorted[random.Next(0, survivorCount)]);
        }

        return result;
    }
}
