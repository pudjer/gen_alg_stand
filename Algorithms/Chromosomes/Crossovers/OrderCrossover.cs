using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.Crossovers;

public sealed class OrderCrossover<TSelf> : CrossoverBase<TSelf>
    where TSelf : IOrderedChromosome<TSelf>
{
    public OrderCrossover(double crossoverProbability = 1.0)
        : base(crossoverProbability)
    {
    }

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
