using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.Mutations;

/// <summary>Swaps two distinct genes of an ordered chromosome.</summary>
/// <typeparam name="TSelf">Concrete ordered chromosome type.</typeparam>
public sealed class SwapMutation<TSelf> : PermutationMutationBase<TSelf>
    where TSelf : IOrderedChromosome<TSelf>
{
    /// <summary>Initializes a new instance of the <see cref="SwapMutation{TSelf}"/> class.</summary>
    /// <param name="chromosomeProbability">Probability in [0, 1] that a chromosome is mutated.</param>
    public SwapMutation(double chromosomeProbability = 0.1)
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
        var offset = random.Next(1, n);
        var j = (i + offset) % n;

        (genes[i], genes[j]) = (genes[j], genes[i]);
    }
}
