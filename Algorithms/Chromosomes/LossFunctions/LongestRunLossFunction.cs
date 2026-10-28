using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.LossFunctions;

/// <summary>
/// Minimization loss that rewards binary chromosomes containing a long consecutive run of true genes.
/// </summary>
/// <typeparam name="TChromosome">The binary chromosome type being evaluated.</typeparam>
public sealed class LongestRunLossFunction<TChromosome> : ILossFunction<TChromosome>
    where TChromosome : IBinaryChromosome<TChromosome>
{
    /// <summary>
    /// Creates the LongestRun loss function.
    /// </summary>
    public LongestRunLossFunction()
    {
    }

    /// <summary>
    /// Returns the negated length of the longest consecutive run of true genes.
    /// </summary>
    /// <param name="chromosome">The binary chromosome to evaluate.</param>
    /// <returns>The negated longest run length, or zero when the chromosome has no genes.</returns>
    public double Evaluate(TChromosome chromosome)
    {
        ArgumentNullException.ThrowIfNull(chromosome);

        int length = chromosome.Length;
        if (length == 0)
        {
            return 0d;
        }

        int currentRun = 0;
        int bestRun = 0;

        for (int i = 0; i < length; i++)
        {
            if (chromosome[i])
            {
                currentRun++;
                if (currentRun > bestRun)
                {
                    bestRun = currentRun;
                }
            }
            else
            {
                currentRun = 0;
            }
        }

        return -bestRun;
    }
}
