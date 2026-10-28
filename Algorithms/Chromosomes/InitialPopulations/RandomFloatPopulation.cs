using System.Collections.Generic;
using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.InitialPopulations;

public sealed class RandomFloatPopulation<TChromosome> : IInitialPopulation<TChromosome>
    where TChromosome : IFloatChromosome<TChromosome>
{
    private readonly int _size;
    private readonly TChromosome _template;

    public RandomFloatPopulation(int size, TChromosome template)
    {
        ArgumentNullException.ThrowIfNull(template);
        ArgumentOutOfRangeException.ThrowIfLessThan(size, 1);

        _size = size;
        _template = template;
    }

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
