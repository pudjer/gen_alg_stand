using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.LossFunctions;

/// <summary>
/// Minimization loss implementing the Booth function over the first two genes.
/// </summary>
/// <typeparam name="TChromosome">The float chromosome type being evaluated.</typeparam>
public sealed class BoothLossFunction<TChromosome> : ILossFunction<TChromosome>
    where TChromosome : IFloatChromosome<TChromosome>
{
    /// <summary>
    /// Creates the Booth loss function.
    /// </summary>
    public BoothLossFunction()
    {
    }

    /// <summary>
    /// Returns the Booth value computed from the first two genes.
    /// </summary>
    /// <param name="chromosome">The float chromosome to evaluate.</param>
    /// <returns>The Booth value, or zero when the chromosome has fewer than two genes.</returns>
    public double Evaluate(TChromosome chromosome)
    {
        ArgumentNullException.ThrowIfNull(chromosome);

        if (chromosome.Length < 2)
        {
            return 0d;
        }

        double x0 = chromosome[0];
        double x1 = chromosome[1];
        double firstTerm = x0 + (2d * x1) - 7d;
        double secondTerm = (2d * x0) + x1 - 5d;

        return (firstTerm * firstTerm) + (secondTerm * secondTerm);
    }
}

/// <summary>
/// Minimization loss implementing the Beale function over the first two genes.
/// </summary>
/// <typeparam name="TChromosome">The float chromosome type being evaluated.</typeparam>
public sealed class BealeLossFunction<TChromosome> : ILossFunction<TChromosome>
    where TChromosome : IFloatChromosome<TChromosome>
{
    /// <summary>
    /// Creates the Beale loss function.
    /// </summary>
    public BealeLossFunction()
    {
    }

    /// <summary>
    /// Returns the Beale value computed from the first two genes.
    /// </summary>
    /// <param name="chromosome">The float chromosome to evaluate.</param>
    /// <returns>The Beale value, or zero when the chromosome has fewer than two genes.</returns>
    public double Evaluate(TChromosome chromosome)
    {
        ArgumentNullException.ThrowIfNull(chromosome);

        if (chromosome.Length < 2)
        {
            return 0d;
        }

        double x0 = chromosome[0];
        double x1 = chromosome[1];
        double firstTerm = 1.5d - x0 + (x0 * x1);
        double secondTerm = 2.25d - x0 + (x0 * x1 * x1);
        double thirdTerm = 2.625d - x0 + (x0 * x1 * x1 * x1);

        return (firstTerm * firstTerm) + (secondTerm * secondTerm) + (thirdTerm * thirdTerm);
    }
}

/// <summary>
/// Minimization loss implementing the Himmelblau function over the first two genes.
/// </summary>
/// <typeparam name="TChromosome">The float chromosome type being evaluated.</typeparam>
public sealed class HimmelblauLossFunction<TChromosome> : ILossFunction<TChromosome>
    where TChromosome : IFloatChromosome<TChromosome>
{
    /// <summary>
    /// Creates the Himmelblau loss function.
    /// </summary>
    public HimmelblauLossFunction()
    {
    }

    /// <summary>
    /// Returns the Himmelblau value computed from the first two genes.
    /// </summary>
    /// <param name="chromosome">The float chromosome to evaluate.</param>
    /// <returns>The Himmelblau value, or zero when the chromosome has fewer than two genes.</returns>
    public double Evaluate(TChromosome chromosome)
    {
        ArgumentNullException.ThrowIfNull(chromosome);

        if (chromosome.Length < 2)
        {
            return 0d;
        }

        double x0 = chromosome[0];
        double x1 = chromosome[1];
        double firstTerm = (x0 * x0) + x1 - 11d;
        double secondTerm = x0 + (x1 * x1) - 7d;

        return (firstTerm * firstTerm) + (secondTerm * secondTerm);
    }
}

/// <summary>
/// Minimization loss implementing the Easom function over the first two genes.
/// </summary>
/// <typeparam name="TChromosome">The float chromosome type being evaluated.</typeparam>
public sealed class EasomLossFunction<TChromosome> : ILossFunction<TChromosome>
    where TChromosome : IFloatChromosome<TChromosome>
{
    /// <summary>
    /// Creates the Easom loss function.
    /// </summary>
    public EasomLossFunction()
    {
    }

    /// <summary>
    /// Returns the Easom value computed from the first two genes.
    /// </summary>
    /// <param name="chromosome">The float chromosome to evaluate.</param>
    /// <returns>The Easom value, or zero when the chromosome has fewer than two genes.</returns>
    public double Evaluate(TChromosome chromosome)
    {
        ArgumentNullException.ThrowIfNull(chromosome);

        if (chromosome.Length < 2)
        {
            return 0d;
        }

        double x0 = chromosome[0];
        double x1 = chromosome[1];
        double offset0 = x0 - Math.PI;
        double offset1 = x1 - Math.PI;
        double exponent = -((offset0 * offset0) + (offset1 * offset1));

        return -Math.Cos(x0) * Math.Cos(x1) * Math.Exp(exponent);
    }
}

/// <summary>
/// Minimization loss implementing the Bukin function over the first two genes.
/// </summary>
/// <typeparam name="TChromosome">The float chromosome type being evaluated.</typeparam>
public sealed class BukinLossFunction<TChromosome> : ILossFunction<TChromosome>
    where TChromosome : IFloatChromosome<TChromosome>
{
    /// <summary>
    /// Creates the Bukin loss function.
    /// </summary>
    public BukinLossFunction()
    {
    }

    /// <summary>
    /// Returns the Bukin value computed from the first two genes.
    /// </summary>
    /// <param name="chromosome">The float chromosome to evaluate.</param>
    /// <returns>The Bukin value, or zero when the chromosome has fewer than two genes.</returns>
    public double Evaluate(TChromosome chromosome)
    {
        ArgumentNullException.ThrowIfNull(chromosome);

        if (chromosome.Length < 2)
        {
            return 0d;
        }

        double x0 = chromosome[0];
        double x1 = chromosome[1];
        double firstTerm = 100d * Math.Sqrt(Math.Abs(x1 - (0.01d * x0 * x0)));
        double secondTerm = 0.01d * Math.Abs(x0 + 10d);

        return firstTerm + secondTerm;
    }
}

/// <summary>
/// Minimization loss implementing the Matyas function over the first two genes.
/// </summary>
/// <typeparam name="TChromosome">The float chromosome type being evaluated.</typeparam>
public sealed class MatyasLossFunction<TChromosome> : ILossFunction<TChromosome>
    where TChromosome : IFloatChromosome<TChromosome>
{
    /// <summary>
    /// Creates the Matyas loss function.
    /// </summary>
    public MatyasLossFunction()
    {
    }

    /// <summary>
    /// Returns the Matyas value computed from the first two genes.
    /// </summary>
    /// <param name="chromosome">The float chromosome to evaluate.</param>
    /// <returns>The Matyas value, or zero when the chromosome has fewer than two genes.</returns>
    public double Evaluate(TChromosome chromosome)
    {
        ArgumentNullException.ThrowIfNull(chromosome);

        if (chromosome.Length < 2)
        {
            return 0d;
        }

        double x0 = chromosome[0];
        double x1 = chromosome[1];

        return (0.26d * ((x0 * x0) + (x1 * x1))) - (0.48d * x0 * x1);
    }
}
