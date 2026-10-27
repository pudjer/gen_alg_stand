using Core.Contracts;

namespace Algorithms.Mutations;

/// <summary>Applies a mutation operator chromosome by chromosome with a configurable probability.</summary>
/// <typeparam name="TSelf">Concrete chromosome type.</typeparam>
public abstract class MutationBase<TSelf> : IMutation<TSelf>
    where TSelf : IChromosome<TSelf>
{
    private readonly double _chromosomeProbability;

    /// <summary>Initializes a new instance of the <see cref="MutationBase{TSelf}"/> class.</summary>
    /// <param name="chromosomeProbability">Probability in [0, 1] that a chromosome is mutated.</param>
    protected MutationBase(double chromosomeProbability = 0.1)
    {
        if (!double.IsFinite(chromosomeProbability) || chromosomeProbability < 0.0 || chromosomeProbability > 1.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(chromosomeProbability),
                chromosomeProbability,
                "The chromosome mutation probability must be a finite value in [0, 1].");
        }

        _chromosomeProbability = chromosomeProbability;
    }

    /// <summary>Gets the probability that a chromosome is mutated.</summary>
    protected double ChromosomeProbability => _chromosomeProbability;

    /// <inheritdoc />
    public IReadOnlyList<TSelf> Mutate(IReadOnlyList<TSelf> population, IRandom random)
    {
        ArgumentNullException.ThrowIfNull(population);
        ArgumentNullException.ThrowIfNull(random);

        if (population.Count == 0)
        {
            return Array.Empty<TSelf>();
        }

        var result = new List<TSelf>(population.Count);

        foreach (var chromosome in population)
        {
            result.Add(random.NextDouble() < _chromosomeProbability
                ? MutateChromosome(chromosome, random)
                : chromosome.Clone());
        }

        return result;
    }

    /// <summary>Mutates a single chromosome.</summary>
    /// <param name="chromosome">Chromosome to mutate; must not be modified.</param>
    /// <param name="random">Random source.</param>
    /// <returns>The mutated chromosome.</returns>
    protected abstract TSelf MutateChromosome(TSelf chromosome, IRandom random);
}
