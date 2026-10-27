using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.Crossovers;

/// <summary>Classic cycle crossover (CX) for ordered chromosomes.</summary>
/// <typeparam name="TSelf">The concrete ordered chromosome type.</typeparam>
/// <remarks>
/// Both parents must contain the same set of genes (typically a permutation of the same values);
/// otherwise an <see cref="ArgumentException"/> is thrown.
/// </remarks>
public sealed class CycleCrossover<TSelf> : CrossoverBase<TSelf>
    where TSelf : IOrderedChromosome<TSelf>
{
    /// <summary>Creates a cycle crossover.</summary>
    /// <param name="crossoverProbability">Probability that a pair is recombined; must be in [0, 1].</param>
    public CycleCrossover(double crossoverProbability = 1.0)
        : base(crossoverProbability)
    {
    }

    /// <summary>Produces one CX offspring pair from two ordered parents sharing the same gene set.</summary>
    /// <param name="first">First parent.</param>
    /// <param name="second">Second parent; must have the same length and gene set as <paramref name="first"/>.</param>
    /// <param name="random">Random source, unused because cycle crossover is deterministic.</param>
    /// <returns>The two children created by cycle crossover.</returns>
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

        // Positions not reached by any cycle (only possible with malformed input).
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

    /// <summary>Maps each gene in <paramref name="parent"/> to its position.</summary>
    private static Dictionary<int, int> BuildPositionLookup(int[] parent)
    {
        Dictionary<int, int> lookup = new Dictionary<int, int>(parent.Length);
        for (int i = 0; i < parent.Length; i++)
        {
            lookup.TryAdd(parent[i], i);
        }

        return lookup;
    }

    /// <summary>Checks whether both arrays contain exactly the same distinct values.</summary>
    private static bool SameGeneSet(int[] first, int[] second)
    {
        HashSet<int> set = new HashSet<int>(first);
        return set.SetEquals(second);
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
