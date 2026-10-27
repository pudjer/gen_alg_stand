using System.Collections.Generic;
using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.InitialPopulations;

/// <summary>
/// Creates an initial population of floating-point chromosomes with uniformly random genes
/// drawn from the range [MinValue, MaxValue] of a template chromosome.
/// </summary>
/// <typeparam name="TChromosome">The floating-point chromosome type.</typeparam>
public sealed class RandomFloatPopulation<TChromosome> : IInitialPopulation<TChromosome>
    where TChromosome : IFloatChromosome<TChromosome>
{
    private readonly int _size;
    private readonly TChromosome _template;

    /// <summary>Initializes a new population generator.</summary>
    /// <param name="size">Number of chromosomes in the population; must be at least 1.</param>
    /// <param name="template">Chromosome supplying gene count and bounds; never mutated.</param>
    /// <exception cref="ArgumentNullException"><paramref name="template"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="size"/> is less than 1.</exception>
    public RandomFloatPopulation(int size, TChromosome template)
    {
        ArgumentNullException.ThrowIfNull(template);
        ArgumentOutOfRangeException.ThrowIfLessThan(size, 1);

        _size = size;
        _template = template;
    }

    /// <summary>Creates exactly <c>size</c> chromosomes with uniform random floating-point genes.</summary>
    /// <param name="random">Random source used for gene values.</param>
    /// <returns>A read-only list of newly created chromosomes.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="random"/> is <see langword="null"/>.</exception>
    public IReadOnlyList<TChromosome> Create(IRandom random)
    {
        ArgumentNullException.ThrowIfNull(random);

        int length = _template.Length;
        double min = _template.MinValue;
        double max = _template.MaxValue;

        var population = new List<TChromosome>(_size);
        for (int i = 0; i < _size; i++)
        {
            var genes = new double[length];
            for (int g = 0; g < genes.Length; g++)
            {
                genes[g] = min + (random.NextDouble() * (max - min));
            }

            population.Add(_template.WithGenes(genes));
        }

        return population;
    }
}
