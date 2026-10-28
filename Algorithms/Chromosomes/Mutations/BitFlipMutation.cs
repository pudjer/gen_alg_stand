using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.Mutations;

public sealed class BitFlipMutation<TSelf> : MutationBase<TSelf>
    where TSelf : IBinaryChromosome<TSelf>
{
    private readonly double _geneProbability;

    public BitFlipMutation(double geneProbability = 0.01, double chromosomeProbability = 0.1)
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
        var genes = new bool[chromosome.Length];

        for (var i = 0; i < genes.Length; i++)
        {
            genes[i] = random.NextDouble() < _geneProbability
                ? !chromosome[i]
                : chromosome[i];
        }

        return chromosome.WithGenes(genes);
    }
}
