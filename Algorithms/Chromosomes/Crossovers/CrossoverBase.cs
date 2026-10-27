using Core.Contracts;

namespace Algorithms.Crossovers;

/// <summary>Pairs adjacent chromosomes and produces one offspring pair per pair.</summary>
/// <typeparam name="TSelf">Concrete chromosome type.</typeparam>
public abstract class CrossoverBase<TSelf> : ICrossover<TSelf>
    where TSelf : IChromosome<TSelf>
{
    private readonly double _crossoverProbability;

    /// <summary>Initializes a new instance of the <see cref="CrossoverBase{TSelf}"/> class.</summary>
    /// <param name="crossoverProbability">Probability in [0, 1] that a pair is recombined.</param>
    protected CrossoverBase(double crossoverProbability = 1.0)
    {
        if (!double.IsFinite(crossoverProbability) || crossoverProbability < 0.0 || crossoverProbability > 1.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(crossoverProbability),
                crossoverProbability,
                "The crossover probability must be a finite value in [0, 1].");
        }

        _crossoverProbability = crossoverProbability;
    }

    /// <summary>Gets the probability that a pair of parents is recombined.</summary>
    protected double CrossoverProbability => _crossoverProbability;

    /// <inheritdoc />
    public IReadOnlyList<TSelf> Crossover(IReadOnlyList<TSelf> population, IRandom random)
    {
        ArgumentNullException.ThrowIfNull(population);
        ArgumentNullException.ThrowIfNull(random);

        if (population.Count == 0)
        {
            return Array.Empty<TSelf>();
        }

        var offspring = new List<TSelf>(population.Count);

        for (var i = 0; i < population.Count; i += 2)
        {
            var first = population[i];

            if (i + 1 >= population.Count)
            {
                offspring.Add(first.Clone());
                break;
            }

            var second = population[i + 1];

            if (random.NextDouble() < _crossoverProbability)
            {
                var (child1, child2) = CrossPair(first, second, random);
                offspring.Add(child1);
                offspring.Add(child2);
            }
            else
            {
                offspring.Add(first.Clone());
                offspring.Add(second.Clone());
            }
        }

        return offspring;
    }

    /// <summary>Crosses two parents and returns the resulting offspring pair.</summary>
    /// <param name="first">First parent.</param>
    /// <param name="second">Second parent.</param>
    /// <param name="random">Random source.</param>
    /// <returns>The two offspring.</returns>
    protected abstract (TSelf First, TSelf Second) CrossPair(TSelf first, TSelf second, IRandom random);
}
