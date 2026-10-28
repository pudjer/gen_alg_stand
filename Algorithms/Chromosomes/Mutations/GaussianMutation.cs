using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.Mutations;

public sealed class GaussianMutation<TSelf> : MutationBase<TSelf>
    where TSelf : IFloatChromosome<TSelf>
{
    private readonly double _geneProbability;
    private readonly double _sigma;

    public GaussianMutation(
        double geneProbability = 0.01,
        double sigma = 0.1,
        double chromosomeProbability = 0.1)
        : base(chromosomeProbability)
    {
        if (!double.IsFinite(geneProbability) || geneProbability < 0.0 || geneProbability > 1.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(geneProbability),
                geneProbability,
                "The gene mutation probability must be a finite value in [0, 1].");
        }

        if (!double.IsFinite(sigma) || sigma < 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sigma),
                sigma,
                "The sigma value must be finite and non-negative.");
        }

        _geneProbability = geneProbability;
        _sigma = sigma;
    }

    protected override TSelf MutateChromosome(TSelf chromosome, IRandom random)
    {
        var genes = new double[chromosome.Length];
        var min = chromosome.MinValue;
        var max = chromosome.MaxValue;
        var std = _sigma * (max - min);

        for (var i = 0; i < genes.Length; i++)
        {
            if (std == 0.0 || random.NextDouble() >= _geneProbability)
            {
                genes[i] = chromosome[i];
                continue;
            }

            var value = chromosome[i] + (NextGaussian(random) * std);
            genes[i] = Math.Clamp(value, min, max);
        }

        return chromosome.WithGenes(genes);
    }

    private static double NextGaussian(IRandom random)
    {
        var u1 = 1.0 - random.NextDouble();
        var u2 = random.NextDouble();
        return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
    }
}
