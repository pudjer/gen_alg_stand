using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.LossFunctions;

public sealed class SphereLossFunction<TChromosome> : ILossFunction<TChromosome>
    where TChromosome : IFloatChromosome<TChromosome>
{
    public SphereLossFunction()
    {
    }

    public double Evaluate(TChromosome chromosome)
    {
        ArgumentNullException.ThrowIfNull(chromosome);

        double sum = 0d;
        for (int i = 0; i < chromosome.Length; i++)
        {
            double value = chromosome[i];
            sum += value * value;
        }

        return sum;
    }
}
