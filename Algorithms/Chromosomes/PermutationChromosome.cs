using System.Collections.ObjectModel;
using Core.Contracts;

namespace Algorithms.Chromosomes;

/// <summary>
/// Immutable chromosome whose genotype is a non-empty permutation of <c>0..Length-1</c>.
/// </summary>
/// <remarks>
/// The incoming sequence is copied on construction, so later mutations of the caller's
/// collection cannot affect this instance. Every value from <c>0</c> to <c>Length - 1</c> must
/// appear exactly once. The underlying array is never exposed; use <see cref="Genes"/> for a
/// read-only view or <see cref="WithGenes"/> to derive a new value.
/// </remarks>
public sealed class PermutationChromosome : IPermutationChromosome<PermutationChromosome>
{
    private readonly int[] _genes;
    private readonly ReadOnlyCollection<int> _genesView;

    /// <summary>
    /// Initializes a new instance of the <see cref="PermutationChromosome"/> class by copying and
    /// validating the supplied genes.
    /// </summary>
    /// <param name="genes">
    /// A permutation of <c>0..genes.Count - 1</c>. Must contain at least one element.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="genes"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="genes"/> is empty, contains a duplicate value, or contains a value outside
    /// the range <c>0..genes.Count - 1</c>.
    /// </exception>
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

    /// <summary>
    /// Gets the number of genes in this chromosome.
    /// </summary>
    public int Length => _genes.Length;

    /// <summary>
    /// Gets the gene at the specified position.
    /// </summary>
    /// <param name="index">The zero-based position of the gene.</param>
    /// <returns>The permutation value stored at <paramref name="index"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="index"/> is negative or greater than or equal to <see cref="Length"/>.
    /// </exception>
    public int this[int index]
    {
        get
        {
            ArgumentOutOfRangeException.ThrowIfNegative(index);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, _genes.Length);

            return _genes[index];
        }
    }

    /// <summary>
    /// Gets a read-only view over the genes of this chromosome.
    /// </summary>
    /// <remarks>
    /// The view reflects the immutable contents of this instance and cannot be used to modify it.
    /// </remarks>
    public IReadOnlyList<int> Genes => _genesView;

    /// <summary>
    /// Creates the identity permutation <c>0, 1, ..., length - 1</c>.
    /// </summary>
    /// <param name="length">The number of genes in the resulting chromosome. Must be at least 1.</param>
    /// <returns>A new <see cref="PermutationChromosome"/> holding the identity permutation.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="length"/> is less than 1.
    /// </exception>
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

    /// <summary>
    /// Creates a new chromosome with the supplied genes.
    /// </summary>
    /// <param name="genes">
    /// A permutation of <c>0..genes.Count - 1</c>. Must contain at least one element.
    /// </param>
    /// <returns>A new <see cref="PermutationChromosome"/> holding a copy of <paramref name="genes"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="genes"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="genes"/> is empty, contains a duplicate value, or contains a value outside
    /// the range <c>0..genes.Count - 1</c>.
    /// </exception>
    public PermutationChromosome WithGenes(IReadOnlyList<int> genes) => new(genes);

    /// <summary>
    /// Creates an independent copy of this chromosome.
    /// </summary>
    /// <returns>A new <see cref="PermutationChromosome"/> with the same genes as this instance.</returns>
    public PermutationChromosome Clone() => new(_genes);
}
