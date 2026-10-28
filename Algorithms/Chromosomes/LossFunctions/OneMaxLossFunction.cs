using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.LossFunctions;

public sealed class OneMaxLossFunction<TChromosome> : ILossFunction<TChromosome>
    where TChromosome : IBinaryChromosome<TChromosome>
{
    public OneMaxLossFunction()
    {
    }

    public double Evaluate(TChromosome chromosome)
    {
        ArgumentNullException.ThrowIfNull(chromosome);

        int count = 0;
        for (int i = 0; i < chromosome.Length; i++)
        {
            if (chromosome[i])
            {
                count++;
            }
        }

        return -count;
    }
}
