using System.Collections.Generic;
using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.InitialPopulations;

/// <summary>
/// Creates an initial population of integer chromosomes with uniformly random genes
/// drawn from the inclusive range [MinValue, MaxValue] of a template chromosome.
/// </summary>
/// <typeparam name="TChromosome">The integer chromosome type.</typeparam>
public sealed class RandomIntPopulation<TChromosome> : IInitialPopulation<TChromosome>
    where TChromosome : IIntChromosome<TChromosome>
{
    private readonly int _size;
    private readonly TChromosome _template;

    /// <summary>Initializes a new population generator.</summary>
    /// <param name="size">Number of chromosomes in the population; must be at least 1.</param>
    /// <param name="template">Chromosome supplying gene count and inclusive bounds; never mutated.</param>
    /// <exception cref="ArgumentNullException"><paramref name="template"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="size"/> is less than 1.</exception>
    public RandomIntPopulation(int size, TChromosome template)
    {
        ArgumentNullException.ThrowIfNull(template);
        ArgumentOutOfRangeException.ThrowIfLessThan(size, 1);

        _size = size;
        _template = template;
    }

    /// <summary>Creates exactly <c>size</c> chromosomes with uniform random integer genes.</summary>
    /// <param name="random">Random source used for gene values.</param>
    /// <returns>A read-only list of newly created chromosomes.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="random"/> is <see langword="null"/>.</exception>
    public IReadOnlyList<TChromosome> Create(IRandom random)
    {
        ArgumentNullException.ThrowIfNull(random);

        int length = _template.Length;
        int min = _template.MinValue;
        int max = _template.MaxValue;

        var population = new List<TChromosome>(_size);
        for (int i = 0; i < _size; i++)
        {
            var genes = new int[length];
            for (int g = 0; g < genes.Length; g++)
            {
                genes[g] = NextInclusive(random, min, max);
            }

            population.Add(_template.WithGenes(genes));
        }

        return population;
    }

    /// <summary>
    /// Returns a uniform integer in [<paramref name="min"/>, <paramref name="max"/>],
    /// falling back to unit-interval scaling when <c>max + 1</c> would overflow.
    /// </summary>
    private static int NextInclusive(IRandom random, int min, int max)
    {
        if (max < int.MaxValue)
        {
            return random.Next(min, max + 1);
        }

        // The exclusive upper bound is not representable, so scale the unit interval
        // instead. The span fits in a long (2^32 for the full range), and the sampled
        // offset is clamped so floating-point rounding can never exceed the range.
        long span = (long)max - min + 1;
        long offset = (long)(random.NextDouble() * span);
        if (offset >= span)
        {
            offset = span - 1;
        }

        return unchecked((int)(min + offset));
    }
}
