using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.LossFunctions;

/// <summary>
/// Minimization loss implementing the Ackley function over all genes.
/// </summary>
/// <typeparam name="TChromosome">The float chromosome type being evaluated.</typeparam>
public sealed class AckleyLossFunction<TChromosome> : ILossFunction<TChromosome>
    where TChromosome : IFloatChromosome<TChromosome>
{
    /// <summary>
    /// Creates the Ackley loss function.
    /// </summary>
    public AckleyLossFunction()
    {
    }

    /// <summary>
    /// Returns the Ackley value computed from the chromosome length and its genes.
    /// </summary>
    /// <param name="chromosome">The float chromosome to evaluate.</param>
    /// <returns>The Ackley value, or zero when the chromosome has no genes.</returns>
    public double Evaluate(TChromosome chromosome)
    {
        ArgumentNullException.ThrowIfNull(chromosome);

        int n = chromosome.Length;
        if (n == 0)
        {
            return 0d;
        }

        double sumSquares = 0d;
        double sumCosines = 0d;

        for (int i = 0; i < n; i++)
        {
            double value = chromosome[i];
            sumSquares += value * value;
            sumCosines += Math.Cos(2d * Math.PI * value);
        }

        double firstTerm = -20d * Math.Exp(-0.2d * Math.Sqrt(sumSquares / n));
        double secondTerm = -Math.Exp(sumCosines / n);

        return firstTerm + secondTerm + 20d + Math.E;
    }
}

/// <summary>
/// Minimization loss implementing the Griewank function over all genes.
/// </summary>
/// <typeparam name="TChromosome">The float chromosome type being evaluated.</typeparam>
public sealed class GriewankLossFunction<TChromosome> : ILossFunction<TChromosome>
    where TChromosome : IFloatChromosome<TChromosome>
{
    /// <summary>
    /// Creates the Griewank loss function.
    /// </summary>
    public GriewankLossFunction()
    {
    }

    /// <summary>
    /// Returns the Griewank value computed from the chromosome length and its genes.
    /// </summary>
    /// <param name="chromosome">The float chromosome to evaluate.</param>
    /// <returns>The Griewank value, or zero when the chromosome has no genes.</returns>
    public double Evaluate(TChromosome chromosome)
    {
        ArgumentNullException.ThrowIfNull(chromosome);

        int n = chromosome.Length;
        if (n == 0)
        {
            return 0d;
        }

        double sumSquares = 0d;
        double productCosines = 1d;

        for (int i = 0; i < n; i++)
        {
            double value = chromosome[i];
            sumSquares += (value * value) / 4000d;
            productCosines *= Math.Cos(value / Math.Sqrt(i + 1));
        }

        return 1d + sumSquares - productCosines;
    }
}

/// <summary>
/// Minimization loss implementing the Schwefel function over all genes.
/// </summary>
/// <typeparam name="TChromosome">The float chromosome type being evaluated.</typeparam>
public sealed class SchwefelLossFunction<TChromosome> : ILossFunction<TChromosome>
    where TChromosome : IFloatChromosome<TChromosome>
{
    /// <summary>
    /// Creates the Schwefel loss function.
    /// </summary>
    public SchwefelLossFunction()
    {
    }

    /// <summary>
    /// Returns the Schwefel value computed from the chromosome length and its genes.
    /// </summary>
    /// <param name="chromosome">The float chromosome to evaluate.</param>
    /// <returns>The Schwefel value, or zero when the chromosome has no genes.</returns>
    public double Evaluate(TChromosome chromosome)
    {
        ArgumentNullException.ThrowIfNull(chromosome);

        int n = chromosome.Length;
        if (n == 0)
        {
            return 0d;
        }

        double sum = 0d;

        for (int i = 0; i < n; i++)
        {
            double value = chromosome[i];
            sum += value * Math.Sin(Math.Sqrt(Math.Abs(value)));
        }

        return 418.9828872724338d * n - sum;
    }
}
