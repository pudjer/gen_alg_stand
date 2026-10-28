using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.Crossovers;

public sealed class UniformCrossover<TSelf, TGene> : CrossoverBase<TSelf>
    where TSelf : IGenotypeChromosome<TSelf, TGene>
{
    private readonly double _swapProbability;

    public UniformCrossover(double swapProbability = 0.5, double crossoverProbability = 1.0)
        : base(crossoverProbability)
    {
        if (!double.IsFinite(swapProbability) || swapProbability < 0.0 || swapProbability > 1.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(swapProbability),
                swapProbability,
                "The swap probability must be a finite value in [0, 1].");
        }

        _swapProbability = swapProbability;
    }

    protected override (TSelf First, TSelf Second) CrossPair(TSelf first, TSelf second, IRandom random)
    {
        if (first.Length != second.Length)
        {
            throw new ArgumentException("Parents must have the same length.");
        }

        var length = first.Length;

        if (length == 0)
        {
            return (first.Clone(), second.Clone());
        }

        var child1 = new TGene[length];
        var child2 = new TGene[length];

        for (var i = 0; i < length; i++)
        {
            if (random.NextDouble() < _swapProbability)
            {
                child1[i] = second.Genes[i];
                child2[i] = first.Genes[i];
            }
            else
            {
                child1[i] = first.Genes[i];
                child2[i] = second.Genes[i];
            }
        }

        return (first.WithGenes(child1), second.WithGenes(child2));
    }
}
