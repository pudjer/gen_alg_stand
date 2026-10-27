using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.Mutations;

/// <summary>Resamples integer genes uniformly, with a configurable per-gene probability.</summary>
/// <typeparam name="TSelf">Concrete integer chromosome type.</typeparam>
public sealed class UniformIntMutation<TSelf> : MutationBase<TSelf>
    where TSelf : IIntChromosome<TSelf>
{
    private readonly double _geneProbability;

    /// <summary>Initializes a new instance of the <see cref="UniformIntMutation{TSelf}"/> class.</summary>
    /// <param name="geneProbability">Probability in [0, 1] that a single gene is resampled.</param>
    /// <param name="chromosomeProbability">Probability in [0, 1] that a chromosome is mutated.</param>
    public UniformIntMutation(double geneProbability = 0.01, double chromosomeProbability = 0.1)
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
        var genes = new int[chromosome.Length];
        var min = chromosome.MinValue;
        var max = chromosome.MaxValue;

        for (var i = 0; i < genes.Length; i++)
        {
            genes[i] = random.NextDouble() < _geneProbability
                ? SampleInclusive(min, max, random)
                : chromosome[i];
        }

        return chromosome.WithGenes(genes);
    }

    /// <summary>Samples an integer uniformly from the inclusive range [<paramref name="min"/>, <paramref name="max"/>].</summary>
    private static int SampleInclusive(int min, int max, IRandom random)
    {
        if (min >= max)
        {
            return min;
        }

        if (max < int.MaxValue)
        {
            return random.Next(min, max + 1);
        }

        var span = (long)max - min + 1;
        var offset = (long)(random.NextDouble() * span);
        var value = min + offset;
        return (int)Math.Clamp(value, (long)min, (long)max);
    }
}
