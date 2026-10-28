using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.Mutations;

public sealed class UniformFloatMutation<TSelf> : MutationBase<TSelf>
    where TSelf : IFloatChromosome<TSelf>
{
    private readonly double _geneProbability;

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

    private static double SampleUniform(double min, double max, IRandom random)
    {
        var value = min + (random.NextDouble() * (max - min));
        return Math.Clamp(value, min, max);
    }
}
