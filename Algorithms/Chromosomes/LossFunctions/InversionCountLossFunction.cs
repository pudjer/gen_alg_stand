using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.LossFunctions;

public sealed class InversionCountLossFunction<TChromosome> : ILossFunction<TChromosome>
    where TChromosome : IPermutationChromosome<TChromosome>
{
    public InversionCountLossFunction()
    {
    }

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
