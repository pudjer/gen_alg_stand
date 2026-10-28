using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.LossFunctions;

public sealed class RastriginLossFunction<TChromosome> : ILossFunction<TChromosome>
    where TChromosome : IFloatChromosome<TChromosome>
{
    public RastriginLossFunction()
    {
    }

    public double Evaluate(TChromosome chromosome)
    {
        ArgumentNullException.ThrowIfNull(chromosome);

        int n = chromosome.Length;
        double sum = 10d * n;

        for (int i = 0; i < n; i++)
        {
            double value = chromosome[i];
            sum += (value * value) - (10d * Math.Cos(2d * Math.PI * value));
        }

        return sum;
    }
}
