using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.LossFunctions;

public sealed class RosenbrockLossFunction<TChromosome> : ILossFunction<TChromosome>
    where TChromosome : IFloatChromosome<TChromosome>
{
    public RosenbrockLossFunction()
    {
    }

    public double Evaluate(TChromosome chromosome)
    {
        ArgumentNullException.ThrowIfNull(chromosome);

        if (chromosome.Length < 2)
        {
            return 0d;
        }

        double sum = 0d;
        for (int i = 0; i < chromosome.Length - 1; i++)
        {
            double current = chromosome[i];
            double next = chromosome[i + 1];
            double term = next - (current * current);
            double other = 1d - current;

            sum += (100d * term * term) + (other * other);
        }

        return sum;
    }
}
