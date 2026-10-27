using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.Crossovers;

/// <summary>Classic partially mapped crossover (PMX) for ordered chromosomes.</summary>
/// <typeparam name="TSelf">The concrete ordered chromosome type.</typeparam>
public sealed class PartiallyMappedCrossover<TSelf> : CrossoverBase<TSelf>
    where TSelf : IOrderedChromosome<TSelf>
{
    /// <summary>Creates a partially mapped crossover.</summary>
    /// <param name="crossoverProbability">Probability that a pair is recombined; must be in [0, 1].</param>
    public PartiallyMappedCrossover(double crossoverProbability = 1.0)
        : base(crossoverProbability)
    {
    }

    /// <summary>Produces one PMX offspring pair from two ordered parents with the same length.</summary>
    /// <param name="first">First parent.</param>
    /// <param name="second">Second parent; must have the same length as <paramref name="first"/>.</param>
    /// <param name="random">Random source used to pick the copied segment.</param>
    /// <returns>The two children created by partially mapped crossover.</returns>
    protected override (TSelf First, TSelf Second) CrossPair(TSelf first, TSelf second, IRandom random)
    {
        if (first.Length != second.Length)
        {
            throw new ArgumentException(
                "Both parents must have the same length.",
                nameof(second));
        }

        int length = first.Length;
        if (length < 2)
        {
            return (first.Clone(), second.Clone());
        }

        int[] parent1 = CopyGenes(first, length);
        int[] parent2 = CopyGenes(second, length);

        int start = random.Next(0, length - 1);
        int end = random.Next(start + 1, length + 1);

        int[] child1 = BuildChild(parent1, parent2, start, end);
        int[] child2 = BuildChild(parent2, parent1, start, end);

        return (first.WithGenes(child1), second.WithGenes(child2));
    }

    /// <summary>Builds one PMX child, copying the segment of <paramref name="segmentParent"/> and mapping the rest.</summary>
    private static int[] BuildChild(int[] segmentParent, int[] otherParent, int start, int end)
    {
        int length = segmentParent.Length;
        int[] child = new int[length];
        HashSet<int> used = new HashSet<int>();
        Dictionary<int, int> mapping = new Dictionary<int, int>();

        for (int i = start; i < end; i++)
        {
            child[i] = segmentParent[i];
            used.Add(segmentParent[i]);
            mapping[segmentParent[i]] = otherParent[i];
        }

        for (int i = 0; i < length; i++)
        {
            if (i >= start && i < end)
            {
                continue;
            }

            int value = otherParent[i];
            int guard = 0;
            while (used.Contains(value) && guard < length)
            {
                if (!mapping.TryGetValue(value, out int mapped))
                {
                    break;
                }

                value = mapped;
                guard++;
            }

            if (used.Contains(value))
            {
                // Defensive fallback for parents that are not permutations of the same set.
                value = segmentParent[i];
            }

            child[i] = value;
            used.Add(value);
        }

        return child;
    }

    /// <summary>Copies the chromosome genes into a fresh array without mutating the source.</summary>
    private static int[] CopyGenes(TSelf chromosome, int length)
    {
        int[] genes = new int[length];
        for (int i = 0; i < length; i++)
        {
            genes[i] = chromosome[i];
        }

        return genes;
    }
}
