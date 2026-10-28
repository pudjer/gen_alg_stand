using Core.Contracts;

namespace Algorithms.Selections;

public sealed class RankSelection<TChromosome> : ISelection<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    public RankSelection()
    {
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

        double[] weights = new double[count];
        double total = 0;
        for (int rank = 0; rank < count; rank++)
        {
            double weight = count - rank;
            weights[rank] = weight;
            total += weight;
        }

        List<Individ<TChromosome>> result = new(count);

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
