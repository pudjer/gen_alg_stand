using System.Collections.ObjectModel;
using Core.Contracts;

namespace Algorithms.Chromosomes;

public sealed class PermutationChromosome : IPermutationChromosome<PermutationChromosome>
{
    private readonly int[] _genes;
    private readonly ReadOnlyCollection<int> _genesView;

    public PermutationChromosome(IReadOnlyList<int> genes)
    {
        ArgumentNullException.ThrowIfNull(genes);

        if (genes.Count == 0)
        {
            throw new ArgumentException("A chromosome must contain at least one gene.", nameof(genes));
        }

        int length = genes.Count;
        bool[] seen = new bool[length];

        for (int i = 0; i < length; i++)
        {
            int gene = genes[i];

            if (gene < 0 || gene >= length)
            {
                throw new ArgumentException(
                    $"Gene value {gene} at index {i} is outside the valid range 0..{length - 1}.",
                    nameof(genes));
            }

            if (seen[gene])
            {
                throw new ArgumentException(
                    $"Gene value {gene} appears more than once; a permutation must contain each value exactly once.",
                    nameof(genes));
            }

            seen[gene] = true;
        }

        _genes = new int[length];
        for (int i = 0; i < length; i++)
        {
            _genes[i] = genes[i];
        }

        _genesView = Array.AsReadOnly(_genes);
    }

    public int Length => _genes.Length;

    public int this[int index]
    {
        get
        {
            ArgumentOutOfRangeException.ThrowIfNegative(index);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, _genes.Length);

            return _genes[index];
        }
    }

    public IReadOnlyList<int> Genes => _genesView;

    public static PermutationChromosome Identity(int length)
    {
        if (length < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(length),
                length,
                "The length of a permutation chromosome must be at least 1.");
        }

        int[] genes = new int[length];
        for (int i = 0; i < length; i++)
        {
            genes[i] = i;
        }

        return new PermutationChromosome(genes);
    }

    public PermutationChromosome WithGenes(IReadOnlyList<int> genes) => new(genes);

    public PermutationChromosome Clone() => new(_genes);
}
