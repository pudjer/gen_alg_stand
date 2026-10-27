using System.Collections.Generic;
using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.InitialPopulations;

/// <summary>
/// Creates an initial population of binary chromosomes whose genes are independently
/// set to <see langword="true"/> with a configurable probability.
/// </summary>
/// <typeparam name="TChromosome">The binary chromosome type.</typeparam>
public sealed class RandomBinaryPopulation<TChromosome> : IInitialPopulation<TChromosome>
    where TChromosome : IBinaryChromosome<TChromosome>
{
    private readonly int _size;
    private readonly TChromosome _template;
    private readonly double _trueProbability;

    /// <summary>Initializes a new population generator.</summary>
    /// <param name="size">Number of chromosomes in the population; must be at least 1.</param>
    /// <param name="template">Chromosome supplying gene count; never mutated.</param>
    /// <param name="trueProbability">Probability of a gene being <see langword="true"/>, in [0, 1].</param>
    /// <exception cref="ArgumentNullException"><paramref name="template"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="size"/> is less than 1, or <paramref name="trueProbability"/> is outside [0, 1].
    /// </exception>
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

    /// <summary>Creates exactly <c>size</c> chromosomes with independently sampled binary genes.</summary>
    /// <param name="random">Random source used for gene values.</param>
    /// <returns>A read-only list of newly created chromosomes.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="random"/> is <see langword="null"/>.</exception>
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
