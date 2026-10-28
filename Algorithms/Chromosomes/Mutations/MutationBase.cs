using Core.Contracts;

namespace Algorithms.Mutations;

public abstract class MutationBase<TSelf> : IMutation<TSelf>
    where TSelf : IChromosome<TSelf>
{
    private readonly double _chromosomeProbability;

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

    protected double ChromosomeProbability => _chromosomeProbability;

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

    protected abstract TSelf MutateChromosome(TSelf chromosome, IRandom random);
}
