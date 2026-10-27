using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.Crossovers;

/// <summary>Arithmetic crossover for integer genotypes, rounding to the nearest gene value.</summary>
/// <typeparam name="TSelf">Concrete integer chromosome type.</typeparam>
public sealed class IntegerArithmeticCrossover<TSelf> : CrossoverBase<TSelf>
    where TSelf : IIntChromosome<TSelf>
{
    private readonly double weight;

    /// <summary>Initializes a new instance of the <see cref="IntegerArithmeticCrossover{TSelf}"/> class.</summary>
    /// <param name="weight">Blend weight applied to the first parent; must be finite and in [0, 1].</param>
    /// <param name="crossoverProbability">Probability that a pair is actually recombined; must be in [0, 1].</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="weight"/> is not finite or lies outside [0, 1].</exception>
    public IntegerArithmeticCrossover(double weight = 0.5, double crossoverProbability = 1.0)
        : base(crossoverProbability)
    {
        if (!double.IsFinite(weight))
        {
            throw new ArgumentOutOfRangeException(nameof(weight), weight, "Weight must be a finite number.");
        }

        if (weight < 0.0 || weight > 1.0)
        {
            throw new ArgumentOutOfRangeException(nameof(weight), weight, "Weight must be in [0, 1].");
        }

        this.weight = weight;
    }

    /// <summary>Produces two rounded arithmetic-combination offspring from the supplied parents.</summary>
    /// <param name="first">First parent.</param>
    /// <param name="second">Second parent; must have the same length as <paramref name="first"/>.</param>
    /// <param name="random">Random source (unused; the blend is deterministic).</param>
    /// <returns>The two offspring chromosomes.</returns>
    /// <exception cref="ArgumentException">The parents have different lengths.</exception>
    protected override (TSelf First, TSelf Second) CrossPair(TSelf first, TSelf second, IRandom random)
    {
        if (first.Length != second.Length)
        {
            throw new ArgumentException("Parents must have the same length.", nameof(second));
        }

        if (first.Length == 0)
        {
            return (first.Clone(), second.Clone());
        }

        int length = first.Length;
        int minValue = first.MinValue;
        int maxValue = first.MaxValue;
        var child1Genes = new int[length];
        var child2Genes = new int[length];

        for (int i = 0; i < length; i++)
        {
            double a = first[i];
            double b = second[i];

            double value1 = weight * a + (1.0 - weight) * b;
            double value2 = weight * b + (1.0 - weight) * a;

            child1Genes[i] = ClampRounded(value1, minValue, maxValue);
            child2Genes[i] = ClampRounded(value2, minValue, maxValue);
        }

        return (first.WithGenes(child1Genes), second.WithGenes(child2Genes));
    }

    /// <summary>Rounds away from zero and clamps a double to the integer gene range without overflow.</summary>
    private static int ClampRounded(double value, int minValue, int maxValue)
    {
        double rounded = Math.Round(value, MidpointRounding.AwayFromZero);

        if (rounded <= minValue)
        {
            return minValue;
        }

        if (rounded >= maxValue)
        {
            return maxValue;
        }

        return (int)rounded;
    }
}
