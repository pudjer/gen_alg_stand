using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.Mutations;

public abstract class PermutationMutationBase<TSelf> : MutationBase<TSelf>
    where TSelf : IOrderedChromosome<TSelf>
{
    protected PermutationMutationBase(double chromosomeProbability = 0.1)
        : base(chromosomeProbability)
    {
    }

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

    protected abstract void MutateOrder(int[] genes, IRandom random);
}
