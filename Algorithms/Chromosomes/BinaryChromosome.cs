using System.Collections.ObjectModel;
using Core.Contracts;

namespace Algorithms.Chromosomes;

public sealed class BinaryChromosome : IBinaryChromosome<BinaryChromosome>
{
    private readonly bool[] _genes;
    private readonly ReadOnlyCollection<bool> _genesView;

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

    public int Length => _genes.Length;

    public bool this[int index]
    {
        get
        {
            ArgumentOutOfRangeException.ThrowIfNegative(index);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, _genes.Length);

            return _genes[index];
        }
    }

    public IReadOnlyList<bool> Genes => _genesView;

    public BinaryChromosome WithGenes(IReadOnlyList<bool> genes) => new(genes);

    public BinaryChromosome Clone() => new(_genes);
}
