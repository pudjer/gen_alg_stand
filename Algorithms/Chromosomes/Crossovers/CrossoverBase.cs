using Core.Contracts;

namespace Algorithms.Crossovers;

public abstract class CrossoverBase<TSelf> : ICrossover<TSelf>
    where TSelf : IChromosome<TSelf>
{
    private readonly double _crossoverProbability;

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

    protected double CrossoverProbability => _crossoverProbability;

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

    protected abstract (TSelf First, TSelf Second) CrossPair(TSelf first, TSelf second, IRandom random);
}
