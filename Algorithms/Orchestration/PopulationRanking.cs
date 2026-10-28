using Core.Contracts;

namespace Algorithms.Orchestration;

internal static class PopulationRanking
{
    internal static Individ<TChromosome>[] Best<TChromosome>(
        IReadOnlyList<Individ<TChromosome>> population,
        int count)
        where TChromosome : IChromosome<TChromosome>
    {
        if (population.Count == 0 || count <= 0)
        {
            return [];
        }

        int take = Math.Min(count, population.Count);

        Individ<TChromosome>[] sorted = [.. population];
        double[] losses = new double[sorted.Length];

        for (int i = 0; i < sorted.Length; i++)
        {
            losses[i] = sorted[i].LossValue;
        }

        Array.Sort(losses, sorted);

        if (take == sorted.Length)
        {
            return sorted;
        }

        Individ<TChromosome>[] best = new Individ<TChromosome>[take];
        Array.Copy(sorted, best, take);
        return best;
    }

    internal static int[] WorstIndexes<TChromosome>(
        IReadOnlyList<Individ<TChromosome>> population,
        int count)
        where TChromosome : IChromosome<TChromosome>
    {
        if (population.Count == 0 || count <= 0)
        {
            return [];
        }

        int take = Math.Min(count, population.Count);

        int[] indexes = new int[population.Count];
        double[] losses = new double[population.Count];

        for (int i = 0; i < population.Count; i++)
        {
            indexes[i] = i;
            losses[i] = population[i].LossValue;
        }

        Array.Sort(losses, indexes);
        Array.Reverse(indexes);

        int[] worst = new int[take];
        Array.Copy(indexes, worst, take);
        return worst;
    }
}
