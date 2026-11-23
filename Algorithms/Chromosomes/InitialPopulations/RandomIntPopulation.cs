using System.Collections.Generic;
using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.InitialPopulations;

public sealed class RandomIntPopulation : IInitialPopulation<IntChromosome>
{
    private readonly int _size;
    private readonly IntChromosome _template;

    public RandomIntPopulation(int size, IntChromosome template)
    {
        ArgumentNullException.ThrowIfNull(template);
        ArgumentOutOfRangeException.ThrowIfLessThan(size, 1);

        _size = size;
        _template = template;
    }

    public IReadOnlyList<IntChromosome> Create(IRandom random)
    {
        ArgumentNullException.ThrowIfNull(random);

        int length = _template.Length;
        int min = _template.MinValue;
        int max = _template.MaxValue;

        var population = new List<IntChromosome>(_size);
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

    private static int NextInclusive(IRandom random, int min, int max)
    {
        if (max < int.MaxValue)
        {
            return random.Next(min, max + 1);
        }

        long span = (long)max - min + 1;
        long offset = (long)(random.NextDouble() * span);
        if (offset >= span)
        {
            offset = span - 1;
        }

        return unchecked((int)(min + offset));
    }
}