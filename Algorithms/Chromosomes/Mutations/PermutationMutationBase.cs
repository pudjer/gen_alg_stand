using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.Mutations;

/// <summary>Base class for mutations that reorder the genes of an ordered chromosome.</summary>
/// <typeparam name="TSelf">Concrete ordered chromosome type.</typeparam>
public abstract class PermutationMutationBase<TSelf> : MutationBase<TSelf>
    where TSelf : IOrderedChromosome<TSelf>
{
    /// <summary>Initializes a new instance of the <see cref="PermutationMutationBase{TSelf}"/> class.</summary>
    /// <param name="chromosomeProbability">Probability in [0, 1] that a chromosome is mutated.</param>
    protected PermutationMutationBase(double chromosomeProbability = 0.1)
        : base(chromosomeProbability)
    {
    }

    /// <inheritdoc />
    protected sealed override TSelf MutateChromosome(TSelf chromosome, IRandom random)
    {
        var genes = new int[chromosome.Length];

        for (var i = 0; i < genes.Length; i++)
        {
            genes[i] = chromosome[i];
        }

        MutateOrder(genes, random);
        return chromosome.WithGenes(genes);
    }

    /// <summary>Applies an order mutation to a copy of the chromosome's genes, in place.</summary>
    /// <param name="genes">Mutable copy of the chromosome's genes.</param>
    /// <param name="random">Random source.</param>
    protected abstract void MutateOrder(int[] genes, IRandom random);
}
