using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.Crossovers;

/// <summary>Blend crossover (BLX-alpha) for floating-point genotypes.</summary>
/// <typeparam name="TSelf">Concrete floating-point chromosome type.</typeparam>
public sealed class BlendCrossover<TSelf> : CrossoverBase<TSelf>
    where TSelf : IFloatChromosome<TSelf>
{
    private readonly double alpha;

    /// <summary>Initializes a new instance of the <see cref="BlendCrossover{TSelf}"/> class.</summary>
    /// <param name="alpha">Extension factor of the blending interval; must be finite and non-negative.</param>
    /// <param name="crossoverProbability">Probability that a pair is actually recombined; must be in [0, 1].</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="alpha"/> is not finite or is negative.</exception>
    public BlendCrossover(double alpha = 0.5, double crossoverProbability = 1.0)
        : base(crossoverProbability)
    {
        if (!double.IsFinite(alpha))
        {
            throw new ArgumentOutOfRangeException(nameof(alpha), alpha, "Alpha must be a finite number.");
        }

        if (alpha < 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(alpha), alpha, "Alpha must be non-negative.");
        }

        this.alpha = alpha;
    }

    /// <summary>Produces two BLX-alpha offspring from the supplied parents.</summary>
    /// <param name="first">First parent.</param>
    /// <param name="second">Second parent; must have the same length as <paramref name="first"/>.</param>
    /// <param name="random">Random source used to draw one independent beta per gene and child.</param>
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
        var child1Genes = new double[length];
        var child2Genes = new double[length];

        double span = 1.0 + 2.0 * alpha;

        for (int i = 0; i < length; i++)
        {
            double a = first[i];
            double b = second[i];

            double beta1 = -alpha + random.NextDouble() * span;
            double beta2 = -alpha + random.NextDouble() * span;

            double value1 = a + beta1 * (b - a);
            double value2 = b + beta2 * (a - b);

            child1Genes[i] = Math.Clamp(value1, first.MinValue, first.MaxValue);
            child2Genes[i] = Math.Clamp(value2, first.MinValue, first.MaxValue);
        }

        return (first.WithGenes(child1Genes), second.WithGenes(child2Genes));
    }
}
