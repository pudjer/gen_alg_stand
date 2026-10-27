using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.LossFunctions;

/// <summary>
/// Minimization loss measuring the absolute difference between the sum of the genes and a target value.
/// </summary>
/// <typeparam name="TChromosome">The integer chromosome type being evaluated.</typeparam>
public sealed class SumTargetLossFunction<TChromosome> : ILossFunction<TChromosome>
    where TChromosome : IIntChromosome<TChromosome>
{
    private readonly long _target;

    /// <summary>
    /// Initializes a new instance with the desired sum of all genes.
    /// </summary>
    /// <param name="target">The target total produced by summing the chromosome genes.</param>
    public SumTargetLossFunction(long target)
    {
        _target = target;
    }

    /// <summary>
    /// Returns the absolute difference between the gene sum and the target, accumulated in double precision.
    /// </summary>
    /// <param name="chromosome">The integer chromosome to evaluate.</param>
    /// <returns>A non-negative loss; zero means the gene sum equals the target.</returns>
    public double Evaluate(TChromosome chromosome)
    {
        ArgumentNullException.ThrowIfNull(chromosome);

        double sum = 0d;
        for (int i = 0; i < chromosome.Length; i++)
        {
            sum += chromosome[i];
        }

        return Math.Abs(sum - _target);
    }
}
