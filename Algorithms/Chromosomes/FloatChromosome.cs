using System.Collections.ObjectModel;
using Core.Contracts;

namespace Algorithms.Chromosomes;

public sealed class FloatChromosome : IFloatChromosome<FloatChromosome>
{
    private readonly double[] _genes;
    private readonly ReadOnlyCollection<double> _genesView;

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

    public int Length => _genes.Length;

    public double MinValue { get; }

    public double MaxValue { get; }

    public IReadOnlyList<double> Genes => _genesView;

    public double this[int index]
    {
        get
        {
            ArgumentOutOfRangeException.ThrowIfNegative(index);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, _genes.Length);

            return _genes[index];
        }
    }

    public FloatChromosome WithGenes(IReadOnlyList<double> genes) => new(genes, MinValue, MaxValue);

    public FloatChromosome Clone() => new(_genes, MinValue, MaxValue);
}
