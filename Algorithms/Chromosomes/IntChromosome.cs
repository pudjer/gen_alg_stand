using System.Collections.ObjectModel;
using Core.Contracts;

namespace Algorithms.Chromosomes;

public sealed class IntChromosome : IIntChromosome<IntChromosome>
{
    private readonly int[] _genes;
    private readonly ReadOnlyCollection<int> _genesView;

    public IntChromosome(IReadOnlyList<int> genes, int minValue, int maxValue)
    {
        ArgumentNullException.ThrowIfNull(genes);

        if (genes.Count == 0)
        {
            throw new ArgumentException("The chromosome must contain at least one gene.", nameof(genes));
        }

        if (minValue > maxValue)
        {
            throw new ArgumentOutOfRangeException(
                nameof(minValue),
                minValue,
                "minValue must be less than or equal to maxValue.");
        }

        _genes = new int[genes.Count];

        for (int i = 0; i < genes.Count; i++)
        {
            int gene = genes[i];

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

    public int MinValue { get; }

    public int MaxValue { get; }

    public IReadOnlyList<int> Genes => _genesView;

    public int this[int index]
    {
        get
        {
            ArgumentOutOfRangeException.ThrowIfNegative(index);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, _genes.Length);

            return _genes[index];
        }
    }

    public IntChromosome WithGenes(IReadOnlyList<int> genes) => new(genes, MinValue, MaxValue);

    public IntChromosome Clone() => new(_genes, MinValue, MaxValue);
}
