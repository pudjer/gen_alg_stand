using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.Crossovers;

/// <summary>Classic order crossover (OX) for ordered chromosomes.</summary>
/// <typeparam name="TSelf">The concrete ordered chromosome type.</typeparam>
public sealed class OrderCrossover<TSelf> : CrossoverBase<TSelf>
    where TSelf : IOrderedChromosome<TSelf>
{
    /// <summary>Creates an order crossover.</summary>
    /// <param name="crossoverProbability">Probability that a pair is recombined; must be in [0, 1].</param>
    public OrderCrossover(double crossoverProbability = 1.0)
        : base(crossoverProbability)
    {
    }

    /// <summary>Produces one OX offspring pair from two ordered parents with the same length.</summary>
    /// <param name="first">First parent.</param>
    /// <param name="second">Second parent; must have the same length as <paramref name="first"/>.</param>
    /// <param name="random">Random source used to pick the copied segment.</param>
    /// <returns>The two children created by order crossover.</returns>
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

    /// <summary>Builds one OX child: a segment of <paramref name="primary"/> plus the remaining genes in <paramref name="donor"/> order.</summary>
    private static int[] BuildChild(int[] primary, int[] donor, int start, int end)
    {
        int length = primary.Length;
        int[] child = new int[length];
        bool[] filled = new bool[length];
        HashSet<int> used = new HashSet<int>();

        for (int i = start; i < end; i++)
        {
            child[i] = primary[i];
            filled[i] = true;
            used.Add(primary[i]);
        }

        int freeSlots = length - (end - start);
        int write = end % length;

        for (int k = 0; k < length && freeSlots > 0; k++)
        {
            int position = (end + k) % length;
            int gene = donor[position];
            if (used.Contains(gene))
            {
                continue;
            }

            while (filled[write])
            {
                write = (write + 1) % length;
            }

            child[write] = gene;
            filled[write] = true;
            used.Add(gene);
            freeSlots--;
        }

        // Only reachable when the parents do not share the same gene set: keep the
        // remaining positions equal to the primary parent so no cell is left at its default.
        for (int i = 0; i < length && freeSlots > 0; i++)
        {
            if (filled[i])
            {
                continue;
            }

            child[i] = primary[i];
            filled[i] = true;
            freeSlots--;
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
