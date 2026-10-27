using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.LossFunctions;

/// <summary>
/// Minimization loss counting attacking queen pairs in the N-Queens permutation encoding.
/// </summary>
/// <typeparam name="TChromosome">The permutation chromosome type being evaluated.</typeparam>
public sealed class NQueensLossFunction<TChromosome> : ILossFunction<TChromosome>
    where TChromosome : IPermutationChromosome<TChromosome>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NQueensLossFunction{TChromosome}"/> class.
    /// </summary>
    public NQueensLossFunction()
    {
    }

    /// <summary>
    /// Counts pairs of queens that share a diagonal; a loss of zero means the board is solved.
    /// </summary>
    /// <param name="chromosome">The permutation chromosome where index is the column and the value is the row.</param>
    /// <returns>The number of attacking queen pairs as a double.</returns>
    public double Evaluate(TChromosome chromosome)
    {
        ArgumentNullException.ThrowIfNull(chromosome);

        int length = chromosome.Length;
        if (length < 2)
        {
            return 0d;
        }

        int attackingPairs = 0;
        for (int i = 0; i < length - 1; i++)
        {
            int rowI = chromosome[i];
            for (int j = i + 1; j < length; j++)
            {
                if (Math.Abs(rowI - chromosome[j]) == j - i)
                {
                    attackingPairs++;
                }
            }
        }

        return attackingPairs;
    }
}
