using System.Collections.ObjectModel;
using Core.Contracts;

namespace Algorithms.Chromosomes;

/// <summary>
/// Immutable chromosome whose genotype is a non-empty sequence of bits.
/// </summary>
/// <remarks>
/// The incoming sequence is copied on construction, so later mutations of the caller's
/// collection cannot affect this instance. The underlying array is never exposed; use
/// <see cref="Genes"/> for a read-only view or <see cref="WithGenes"/> to derive a new value.
/// </remarks>
public sealed class BinaryChromosome : IBinaryChromosome<BinaryChromosome>
{
    private readonly bool[] _genes;
    private readonly ReadOnlyCollection<bool> _genesView;

    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryChromosome"/> class by copying the
    /// supplied genes.
    /// </summary>
    /// <param name="genes">The bits that make up the genotype. Must contain at least one element.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="genes"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="genes"/> is empty.
    /// </exception>
    public BinaryChromosome(IReadOnlyList<bool> genes)
    {
        ArgumentNullException.ThrowIfNull(genes);

        if (genes.Count == 0)
        {
            throw new ArgumentException("A chromosome must contain at least one gene.", nameof(genes));
        }

        _genes = new bool[genes.Count];
        for (int i = 0; i < genes.Count; i++)
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
    /// <returns>The bit stored at <paramref name="index"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="index"/> is negative or greater than or equal to <see cref="Length"/>.
    /// </exception>
    public bool this[int index]
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
    public IReadOnlyList<bool> Genes => _genesView;

    /// <summary>
    /// Creates a new chromosome with the supplied genes.
    /// </summary>
    /// <param name="genes">The genes of the new chromosome. Must contain at least one element.</param>
    /// <returns>A new <see cref="BinaryChromosome"/> holding a copy of <paramref name="genes"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="genes"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="genes"/> is empty.
    /// </exception>
    public BinaryChromosome WithGenes(IReadOnlyList<bool> genes) => new(genes);

    /// <summary>
    /// Creates an independent copy of this chromosome.
    /// </summary>
    /// <returns>A new <see cref="BinaryChromosome"/> with the same genes as this instance.</returns>
    public BinaryChromosome Clone() => new(_genes);
}
