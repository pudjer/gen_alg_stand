using System.Collections.ObjectModel;
using Core.Contracts;

namespace Algorithms.Chromosomes;

/// <summary>
/// An immutable chromosome whose genes are double-precision values constrained to a closed interval.
/// </summary>
public sealed class FloatChromosome : IFloatChromosome<FloatChromosome>
{
    private readonly double[] _genes;
    private readonly ReadOnlyCollection<double> _genesView;

    /// <summary>
    /// Initializes a new instance of the <see cref="FloatChromosome"/> class.
    /// </summary>
    /// <param name="genes">The gene values. The sequence is copied; it must be non-empty and finite.</param>
    /// <param name="minValue">The inclusive lower bound for every gene. Must be finite.</param>
    /// <param name="maxValue">The inclusive upper bound for every gene. Must be finite.</param>
    /// <exception cref="ArgumentNullException"><paramref name="genes"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="genes"/> is empty.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="minValue"/> or <paramref name="maxValue"/> is not finite,
    /// <paramref name="minValue"/> is greater than <paramref name="maxValue"/>,
    /// or a gene is not finite or lies outside the interval.
    /// </exception>
    public FloatChromosome(IReadOnlyList<double> genes, double minValue, double maxValue)
    {
        ArgumentNullException.ThrowIfNull(genes);

        if (genes.Count == 0)
        {
            throw new ArgumentException("The chromosome must contain at least one gene.", nameof(genes));
        }

        if (!double.IsFinite(minValue))
        {
            throw new ArgumentOutOfRangeException(
                nameof(minValue),
                minValue,
                "minValue must be a finite number.");
        }

        if (!double.IsFinite(maxValue))
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxValue),
                maxValue,
                "maxValue must be a finite number.");
        }

        if (minValue > maxValue)
        {
            throw new ArgumentOutOfRangeException(
                nameof(minValue),
                minValue,
                "minValue must be less than or equal to maxValue.");
        }

        _genes = new double[genes.Count];

        for (int i = 0; i < genes.Count; i++)
        {
            double gene = genes[i];

            if (!double.IsFinite(gene))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(genes),
                    gene,
                    $"Gene at index {i} must be a finite number.");
            }

            if (gene < minValue || gene > maxValue)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(genes),
                    gene,
                    $"Gene at index {i} must be within [{minValue}, {maxValue}].");
            }

            _genes[i] = gene;
        }

        MinValue = minValue;
        MaxValue = maxValue;
        _genesView = Array.AsReadOnly(_genes);
    }

    /// <summary>
    /// Gets the number of genes in the chromosome.
    /// </summary>
    public int Length => _genes.Length;

    /// <summary>
    /// Gets the inclusive lower bound for every gene.
    /// </summary>
    public double MinValue { get; }

    /// <summary>
    /// Gets the inclusive upper bound for every gene.
    /// </summary>
    public double MaxValue { get; }

    /// <summary>
    /// Gets a read-only view over the genes of the chromosome.
    /// </summary>
    public IReadOnlyList<double> Genes => _genesView;

    /// <summary>
    /// Gets the gene at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the gene.</param>
    /// <returns>The gene value at <paramref name="index"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is outside the bounds of the chromosome.</exception>
    public double this[int index]
    {
        get
        {
            ArgumentOutOfRangeException.ThrowIfNegative(index);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, _genes.Length);

            return _genes[index];
        }
    }

    /// <summary>
    /// Creates a new chromosome that uses the specified genes and keeps the current bounds.
    /// </summary>
    /// <param name="genes">The gene values for the new chromosome.</param>
    /// <returns>A new <see cref="FloatChromosome"/> instance.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="genes"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="genes"/> is empty.</exception>
    /// <exception cref="ArgumentOutOfRangeException">A gene is not finite or lies outside the interval.</exception>
    public FloatChromosome WithGenes(IReadOnlyList<double> genes) => new(genes, MinValue, MaxValue);

    /// <summary>
    /// Creates an independent copy of this chromosome.
    /// </summary>
    /// <returns>A new <see cref="FloatChromosome"/> with the same genes and bounds.</returns>
    public FloatChromosome Clone() => new(_genes, MinValue, MaxValue);
}
