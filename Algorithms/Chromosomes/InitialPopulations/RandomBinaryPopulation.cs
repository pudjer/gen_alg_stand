using System.Collections.Generic;
using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.InitialPopulations;

public sealed class RandomBinaryPopulation<TChromosome> : IInitialPopulation<TChromosome>
    where TChromosome : IBinaryChromosome<TChromosome>
{
    private readonly int _size;
    private readonly TChromosome _template;
    private readonly double _trueProbability;

    public RandomBinaryPopulation(int size, TChromosome template, double trueProbability = 0.5)
    {
        ArgumentNullException.ThrowIfNull(template);
        ArgumentOutOfRangeException.ThrowIfLessThan(size, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(trueProbability, 0.0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(trueProbability, 1.0);

        _size = size;
        _template = template;
        _trueProbability = trueProbability;
    }

    public IReadOnlyList<TChromosome> Create(IRandom random)
    {
        ArgumentNullException.ThrowIfNull(random);

        int length = _template.Length;

        var population = new List<TChromosome>(_size);
        for (int i = 0; i < _size; i++)
        {
            var genes = new bool[length];
            for (int g = 0; g < genes.Length; g++)
            {
                genes[g] = random.NextDouble() < _trueProbability;
            }

            population.Add(_template.WithGenes(genes));
        }

        return population;
    }
}
