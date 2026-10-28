using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.LossFunctions;

public sealed class LongestRunLossFunction<TChromosome> : ILossFunction<TChromosome>
    where TChromosome : IBinaryChromosome<TChromosome>
{
    public LongestRunLossFunction()
    {
    }

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
