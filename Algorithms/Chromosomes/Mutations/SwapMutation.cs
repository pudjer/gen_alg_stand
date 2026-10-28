using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.Mutations;

public sealed class SwapMutation<TSelf> : PermutationMutationBase<TSelf>
    where TSelf : IOrderedChromosome<TSelf>
{
    public SwapMutation(double chromosomeProbability = 0.1)
        : base(chromosomeProbability)
    {
    }

    protected override void MutateOrder(int[] genes, IRandom random)
    {
        var n = genes.Length;
        if (n < 2)
        {
            return;
        }

        var i = random.Next(0, n);
        var offset = random.Next(1, n);
        var j = (i + offset) % n;

        (genes[i], genes[j]) = (genes[j], genes[i]);
    }
}
