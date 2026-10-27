using System.Collections.Generic;
using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.InitialPopulations;

/// <summary>
/// Creates an initial population of permutation chromosomes, each being an independent
/// Fisher-Yates shuffle of the gene values 0..Length-1.
/// </summary>
/// <typeparam name="TChromosome">The permutation chromosome type.</typeparam>
public sealed class RandomPermutationPopulation<TChromosome> : IInitialPopulation<TChromosome>
    where TChromosome : IPermutationChromosome<TChromosome>
{
    private readonly int _size;
    private readonly TChromosome _template;

    /// <summary>Initializes a new population generator.</summary>
    /// <param name="size">Number of chromosomes in the population; must be at least 1.</param>
    /// <param name="template">Chromosome supplying gene count; never mutated.</param>
    /// <exception cref="ArgumentNullException"><paramref name="template"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="size"/> is less than 1.</exception>
    public RandomPermutationPopulation(int size, TChromosome template)
    {
        ArgumentNullException.ThrowIfNull(template);
        ArgumentOutOfRangeException.ThrowIfLessThan(size, 1);

        _size = size;
        _template = template;
    }

    /// <summary>Creates exactly <c>size</c> chromosomes, each a uniformly shuffled permutation.</summary>
    /// <param name="random">Random source used for the shuffles.</param>
    /// <returns>A read-only list of newly created chromosomes.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="random"/> is <see langword="null"/>.</exception>
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
