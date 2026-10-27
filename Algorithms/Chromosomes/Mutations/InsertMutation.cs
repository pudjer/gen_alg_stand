using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.Mutations;

/// <summary>Removes a gene from an ordered chromosome and inserts it at another position.</summary>
/// <typeparam name="TSelf">Concrete ordered chromosome type.</typeparam>
public sealed class InsertMutation<TSelf> : PermutationMutationBase<TSelf>
    where TSelf : IOrderedChromosome<TSelf>
{
    /// <summary>Initializes a new instance of the <see cref="InsertMutation{TSelf}"/> class.</summary>
    /// <param name="chromosomeProbability">Probability in [0, 1] that a chromosome is mutated.</param>
    public InsertMutation(double chromosomeProbability = 0.1)
        : base(chromosomeProbability)
    {
    }

    /// <inheritdoc />
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
