using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.Crossovers;

/// <summary>Two-point crossover: genes between two cuts are exchanged, outer genes are kept.</summary>
/// <typeparam name="TSelf">Concrete genotype chromosome type.</typeparam>
/// <typeparam name="TGene">Gene type.</typeparam>
public sealed class TwoPointCrossover<TSelf, TGene> : CrossoverBase<TSelf>
    where TSelf : IGenotypeChromosome<TSelf, TGene>
{
    /// <summary>Initializes a new instance of the <see cref="TwoPointCrossover{TSelf, TGene}"/> class.</summary>
    /// <param name="crossoverProbability">Probability in [0, 1] that a pair is recombined.</param>
    public TwoPointCrossover(double crossoverProbability = 1.0)
        : base(crossoverProbability)
    {
    }

    /// <inheritdoc />
    protected override (TSelf First, TSelf Second) CrossPair(TSelf first, TSelf second, IRandom random)
    {
        if (first.Length != second.Length)
        {
            throw new ArgumentException("Parents must have the same length.");
        }

        if (first.Length < 2)
        {
            return (first.Clone(), second.Clone());
        }

        var length = first.Length;
        var cut1 = random.Next(1, length);
        var cut2 = random.Next(cut1 + 1, length + 1);

        var child1 = new TGene[length];
        var child2 = new TGene[length];

        for (var i = 0; i < length; i++)
        {
            if (i >= cut1 && i < cut2)
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
