using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.LossFunctions;

public sealed class NQueensLossFunction : ILossFunction<PermutationChromosome>
{
    public NQueensLossFunction()
    {
    }

    public double Evaluate(PermutationChromosome chromosome)
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
