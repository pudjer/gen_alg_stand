using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.Crossovers;

public sealed class CycleCrossover<TSelf> : CrossoverBase<TSelf>
    where TSelf : IOrderedChromosome<TSelf>
{
    public CycleCrossover(double crossoverProbability = 1.0)
        : base(crossoverProbability)
    {
    }

    protected override (TSelf First, TSelf Second) CrossPair(TSelf first, TSelf second, IRandom random)
    {
        if (first.Length != second.Length)
        {
            throw new ArgumentException(
                "Cycle crossover requires both parents to have the same length.",
                nameof(second));
        }

        int length = first.Length;
        if (length < 2)
        {
            return (first.Clone(), second.Clone());
        }

        int[] parent1 = CopyGenes(first, length);
        int[] parent2 = CopyGenes(second, length);

        if (!SameGeneSet(parent1, parent2))
        {
            throw new ArgumentException(
                "Cycle crossover requires both parents to contain the same gene set.",
                nameof(second));
        }

        Dictionary<int, int> positionsInParent2 = BuildPositionLookup(parent2);
        int[] child1 = new int[length];
        int[] child2 = new int[length];
        bool[] assigned = new bool[length];
        int[] cycleMarks = new int[length];
        List<int> cycle = new List<int>(length);
        int cycleIndex = 0;

        for (int seed = 0; seed < length; seed++)
        {
            if (assigned[seed])
            {
                continue;
            }

            cycle.Clear();
            int cycleId = cycleIndex + 1;
            int position = seed;
            int steps = 0;

            while (steps <= length)
            {
                if (assigned[position] || cycleMarks[position] == cycleId)
                {
                    break;
                }

                cycle.Add(position);
                cycleMarks[position] = cycleId;
                steps++;

                int next;
                if (!positionsInParent2.TryGetValue(parent1[position], out next))
                {
                    break;
                }

                if (next == seed)
                {
                    break;
                }

                position = next;
            }

            bool useFirstParent = cycleIndex % 2 == 0;
            foreach (int index in cycle)
            {
                if (assigned[index])
                {
                    continue;
                }

                assigned[index] = true;
                if (useFirstParent)
                {
                    child1[index] = parent1[index];
                    child2[index] = parent2[index];
                }
                else
                {
                    child1[index] = parent2[index];
                    child2[index] = parent1[index];
                }
            }

            cycleIndex++;
        }

        for (int i = 0; i < length; i++)
        {
            if (assigned[i])
            {
                continue;
            }

            child1[i] = parent1[i];
            child2[i] = parent2[i];
        }

        return (first.WithGenes(child1), second.WithGenes(child2));
    }

    private static Dictionary<int, int> BuildPositionLookup(int[] parent)
    {
        Dictionary<int, int> lookup = new Dictionary<int, int>(parent.Length);
        for (int i = 0; i < parent.Length; i++)
        {
            lookup.TryAdd(parent[i], i);
        }

        return lookup;
    }

    private static bool SameGeneSet(int[] first, int[] second)
    {
        HashSet<int> set = new HashSet<int>(first);
        return set.SetEquals(second);
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
