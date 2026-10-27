using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.Mutations;

/// <summary>Flips each binary gene independently with a configurable probability.</summary>
/// <typeparam name="TSelf">Concrete binary chromosome type.</typeparam>
public sealed class BitFlipMutation<TSelf> : MutationBase<TSelf>
    where TSelf : IBinaryChromosome<TSelf>
{
    private readonly double _geneProbability;

    /// <summary>Initializes a new instance of the <see cref="BitFlipMutation{TSelf}"/> class.</summary>
    /// <param name="geneProbability">Probability in [0, 1] that a single gene is flipped.</param>
    /// <param name="chromosomeProbability">Probability in [0, 1] that a chromosome is mutated.</param>
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

    /// <inheritdoc />
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
