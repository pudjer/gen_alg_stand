using Core.Contracts;

namespace Algorithms.Selections;

public sealed class RandomSelection<TChromosome> : ISelection<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    public RandomSelection()
    {
    }

    public IReadOnlyList<TChromosome> Select(IReadOnlyList<TChromosome> population, IRandom random)
    {
        ArgumentNullException.ThrowIfNull(population);
        ArgumentNullException.ThrowIfNull(random);

        int count = population.Count;
        if (count == 0)
        {
            return Array.Empty<TChromosome>();
        }

        List<TChromosome> result = new(count);
        for (int i = 0; i < count; i++)
        {
            result.Add(population[random.Next(0, count)]);
        }

        return result;
    }
}
