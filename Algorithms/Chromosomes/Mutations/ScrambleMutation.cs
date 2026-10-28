using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.Mutations;

public sealed class ScrambleMutation<TSelf> : PermutationMutationBase<TSelf>
    where TSelf : IOrderedChromosome<TSelf>
{
    public ScrambleMutation(double chromosomeProbability = 0.1)
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

        for (var k = end; k > start; k--)
        {
            var t = random.Next(start, k + 1);
            (genes[k], genes[t]) = (genes[t], genes[k]);
        }
    }
}
