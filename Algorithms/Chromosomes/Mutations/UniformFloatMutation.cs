using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.Mutations;

/// <summary>Resamples floating-point genes uniformly, with a configurable per-gene probability.</summary>
/// <typeparam name="TSelf">Concrete floating-point chromosome type.</typeparam>
public sealed class UniformFloatMutation<TSelf> : MutationBase<TSelf>
    where TSelf : IFloatChromosome<TSelf>
{
    private readonly double _geneProbability;

    /// <summary>Initializes a new instance of the <see cref="UniformFloatMutation{TSelf}"/> class.</summary>
    /// <param name="geneProbability">Probability in [0, 1] that a single gene is resampled.</param>
    /// <param name="chromosomeProbability">Probability in [0, 1] that a chromosome is mutated.</param>
    public UniformFloatMutation(double geneProbability = 0.01, double chromosomeProbability = 0.1)
        : base(chromosomeProbability)
    {
        if (!double.IsFinite(geneProbability) || geneProbability < 0.0 || geneProbability > 1.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(geneProbability),
                geneProbability,
                "The gene mutation probability must be a finite value in [0, 1].");
        }

        _geneProbability = geneProbability;
    }

    /// <inheritdoc />
    protected override TSelf MutateChromosome(TSelf chromosome, IRandom random)
    {
        var genes = new double[chromosome.Length];
        var min = chromosome.MinValue;
        var max = chromosome.MaxValue;

        for (var i = 0; i < genes.Length; i++)
        {
            genes[i] = random.NextDouble() < _geneProbability
                ? SampleUniform(min, max, random)
                : chromosome[i];
        }

        return chromosome.WithGenes(genes);
    }

    /// <summary>Samples a value uniformly from [<paramref name="min"/>, <paramref name="max"/>], clamped to the bounds.</summary>
    private static double SampleUniform(double min, double max, IRandom random)
    {
        var value = min + (random.NextDouble() * (max - min));
        return Math.Clamp(value, min, max);
    }
}
