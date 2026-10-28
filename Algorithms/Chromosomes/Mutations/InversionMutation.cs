using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.Mutations;

public sealed class InversionMutation<TSelf> : PermutationMutationBase<TSelf>
    where TSelf : IOrderedChromosome<TSelf>
{
    public InversionMutation(double chromosomeProbability = 0.1)
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

        var start = random.Next(0, n - 1);
        var end = random.Next(start + 1, n);

        for (int left = start, right = end; left < right; left++, right--)
        {
            (genes[left], genes[right]) = (genes[right], genes[left]);
        }
    }
}
