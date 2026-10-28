using System.Collections.Generic;
using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.InitialPopulations;

public sealed class RandomPermutationPopulation<TChromosome> : IInitialPopulation<TChromosome>
    where TChromosome : IPermutationChromosome<TChromosome>
{
    private readonly int _size;
    private readonly TChromosome _template;

    public RandomPermutationPopulation(int size, TChromosome template)
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

        var population = new List<TChromosome>(_size);
        for (int i = 0; i < _size; i++)
        {
            var genes = new int[length];
            for (int g = 0; g < genes.Length; g++)
            {
                genes[g] = g;
            }

            for (int n = genes.Length - 1; n > 0; n--)
            {
                int j = random.Next(0, n + 1);
                (genes[n], genes[j]) = (genes[j], genes[n]);
            }

            population.Add(_template.WithGenes(genes));
        }

        return population;
    }
}
