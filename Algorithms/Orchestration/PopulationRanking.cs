using Core.Contracts;

namespace Algorithms.Orchestration;

/// <summary>
/// Loss-based ordering helpers shared by the orchestration components.
/// </summary>
/// <remarks>
/// Ordering is pure scheduling: the score itself is always delegated to the problem's
/// <see cref="ILossFunction{TChromosome}"/>, and nothing here reinterprets or rescales it.
/// Keeping this in one place stops the elitism and migration code paths from drifting apart.
/// </remarks>
internal static class PopulationRanking
{
    /// <summary>Returns the <paramref name="count"/> lowest-loss chromosomes, best first.</summary>
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

    /// <summary>Returns the indexes of the <paramref name="count"/> highest-loss chromosomes.</summary>
    /// <remarks>
    /// Indexes rather than chromosomes are returned because the callers overwrite these slots, and
    /// the same chromosomes can legitimately appear more than once in a population.
    /// </remarks>
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
