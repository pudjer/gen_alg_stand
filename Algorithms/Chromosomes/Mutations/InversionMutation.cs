using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.Mutations;

/// <summary>Reverses a contiguous, non-empty segment of an ordered chromosome.</summary>
/// <typeparam name="TSelf">Concrete ordered chromosome type.</typeparam>
public sealed class InversionMutation<TSelf> : PermutationMutationBase<TSelf>
    where TSelf : IOrderedChromosome<TSelf>
{
    /// <summary>Initializes a new instance of the <see cref="InversionMutation{TSelf}"/> class.</summary>
    /// <param name="chromosomeProbability">Probability in [0, 1] that a chromosome is mutated.</param>
    public InversionMutation(double chromosomeProbability = 0.1)
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

        var start = random.Next(0, n - 1);
        var end = random.Next(start + 1, n);

        for (int left = start, right = end; left < right; left++, right--)
        {
            (genes[left], genes[right]) = (genes[right], genes[left]);
        }
    }
}
