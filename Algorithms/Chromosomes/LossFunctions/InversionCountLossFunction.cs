using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.LossFunctions;

/// <summary>
/// Minimization loss counting inverted gene pairs, the Kendall-tau distance to the identity permutation.
/// </summary>
/// <typeparam name="TChromosome">The permutation chromosome type being evaluated.</typeparam>
public sealed class InversionCountLossFunction<TChromosome> : ILossFunction<TChromosome>
    where TChromosome : IPermutationChromosome<TChromosome>
{
    /// <summary>
    /// Creates the InversionCount loss function.
    /// </summary>
    public InversionCountLossFunction()
    {
    }

    /// <summary>
    /// Returns the number of inverted pairs in the permutation; a sorted permutation yields zero.
    /// </summary>
    /// <param name="chromosome">The permutation chromosome to evaluate.</param>
    /// <returns>The count of inverted pairs as a double.</returns>
    public double Evaluate(TChromosome chromosome)
    {
        ArgumentNullException.ThrowIfNull(chromosome);

        int length = chromosome.Length;
        if (length < 2)
        {
            return 0d;
        }

        int inversions = 0;
        for (int i = 0; i < length - 1; i++)
        {
            int current = chromosome[i];
            for (int j = i + 1; j < length; j++)
            {
                if (current > chromosome[j])
                {
                    inversions++;
                }
            }
        }

        return inversions;
    }
}
