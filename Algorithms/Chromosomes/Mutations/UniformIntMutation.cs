using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.Mutations;

public sealed class UniformIntMutation<TSelf> : MutationBase<TSelf>
    where TSelf : IIntChromosome<TSelf>
{
    private readonly double _geneProbability;

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
