using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.Mutations;

public sealed class InsertMutation<TSelf> : PermutationMutationBase<TSelf>
    where TSelf : IOrderedChromosome<TSelf>
{
    public InsertMutation(double chromosomeProbability = 0.1)
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
        var j = (i + random.Next(1, n)) % n;
        var gene = genes[i];

        if (i < j)
        {
            for (var k = i; k < j; k++)
            {
                genes[k] = genes[k + 1];
            }
        }
        else
        {
            for (var k = i; k > j; k--)
            {
                genes[k] = genes[k - 1];
            }
        }

        genes[j] = gene;
    }
}
