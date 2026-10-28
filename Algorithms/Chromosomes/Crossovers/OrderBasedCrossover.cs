using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.Crossovers;

/// <summary>Order-based crossover (OX2) for ordered chromosomes.</summary>
/// <typeparam name="TSelf">The concrete ordered chromosome type.</typeparam>
public sealed class OrderBasedCrossover<TSelf> : CrossoverBase<TSelf>
    where TSelf : IOrderedChromosome<TSelf>
{
    /// <summary>Creates an order-based crossover.</summary>
    /// <param name="crossoverProbability">Probability that a pair is recombined; must be in [0, 1].</param>
    public OrderBasedCrossover(double crossoverProbability = 1.0)
        : base(crossoverProbability)
    {
    }

    /// <summary>Produces one OX2 offspring pair from two ordered parents with the same length.</summary>
    /// <param name="first">First parent.</param>
    /// <param name="second">Second parent; must have the same length as <paramref name="first"/>.</param>
    /// <param name="random">Random source used to pick the copied positions.</param>
    /// <returns>The two children created by order-based crossover.</returns>
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
        bool[] selectedPositions = SelectPositions(length, random);

        int[] child1 = BuildChild(parent1, parent2, selectedPositions);
        int[] child2 = BuildChild(parent2, parent1, selectedPositions);

        return (first.WithGenes(child1), second.WithGenes(child2));
    }

    /// <summary>Builds one OX2 child by preserving selected primary genes and filling the rest in donor order.</summary>
    private static int[] BuildChild(int[] primary, int[] donor, bool[] selectedPositions)
    {
        int length = primary.Length;
        int[] child = new int[length];
        bool[] filled = new bool[length];
        HashSet<int> used = new HashSet<int>();

        for (int i = 0; i < length; i++)
        {
            if (selectedPositions[i])
            {
                child[i] = primary[i];
                filled[i] = true;
                used.Add(primary[i]);
            }
        }

        int write = 0;
        for (int i = 0; i < length; i++)
        {
            int gene = donor[i];
            if (used.Contains(gene))
            {
                continue;
            }

            while (filled[write])
            {
                write++;
            }

            child[write] = gene;
            filled[write] = true;
            used.Add(gene);
        }

        return child;
    }

    /// <summary>Selects a random non-empty subset of positions.</summary>
    private static bool[] SelectPositions(int length, IRandom random)
    {
        int[] positions = new int[length];
        for (int i = 0; i < length; i++)
        {
            positions[i] = i;
        }

        for (int i = length - 1; i > 0; i--)
        {
            int swapIndex = random.Next(0, i + 1);
            (positions[i], positions[swapIndex]) = (positions[swapIndex], positions[i]);
        }

        int selectedCount = random.Next(1, length);
        bool[] selected = new bool[length];
        for (int i = 0; i < selectedCount; i++)
        {
            selected[positions[i]] = true;
        }

        return selected;
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
