using Core.Contracts;

namespace Algorithms.Orchestration;

internal static class PopulationRanking
{
    internal static TChromosome[] Best<TChromosome>(
        ILossFunction<TChromosome> lossFunction,
        IReadOnlyList<TChromosome> population,
        int count)
        where TChromosome : IChromosome<TChromosome>
    {
        if (population.Count == 0 || count <= 0)
        {
            return [];
        }

        int take = Math.Min(count, population.Count);

        TChromosome[] sorted = [.. population];
        double[] losses = new double[sorted.Length];

        for (int i = 0; i < sorted.Length; i++)
        {
            losses[i] = lossFunction.Evaluate(sorted[i]);
        }

        Array.Sort(losses, sorted);

        if (take == sorted.Length)
        {
            return sorted;
        }

        TChromosome[] best = new TChromosome[take];
        Array.Copy(sorted, best, take);
        return best;
    }

    internal static int[] WorstIndexes<TChromosome>(
        ILossFunction<TChromosome> lossFunction,
        IReadOnlyList<TChromosome> population,
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
            losses[i] = lossFunction.Evaluate(population[i]);
        }

        Array.Sort(losses, indexes);
        Array.Reverse(indexes);

        int[] worst = new int[take];
        Array.Copy(indexes, worst, take);
        return worst;
    }
}
