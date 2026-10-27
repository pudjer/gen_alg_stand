using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.LossFunctions;

/// <summary>
/// Minimization loss equal to the sum of the squared genes (the Sphere function).
/// </summary>
public sealed class SphereLossFunction<TChromosome> : ILossFunction<TChromosome>
    where TChromosome : IFloatChromosome<TChromosome>
{
    /// <summary>
    /// Creates the Sphere loss function.
    /// </summary>
    public SphereLossFunction()
    {
    }

    /// <summary>
    /// Returns the sum of the squares of all genes.
    /// </summary>
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
