using System.Collections;
using Core.Contracts.Inner;
using Core.Contracts.Outer;

namespace Core.Algorithm;

/// <summary>
/// Популяция поколения: хромосомы и их оценки хранятся параллельно,
/// поэтому обращение к фитнесу — O(1) без словарей и упаковки.
/// <para>
/// Списки копируются при создании: после оценки движок владеет данными
/// безраздельно, и правка списка снаружи не разъедет оценки с хромосомами.
/// </para>
/// </summary>
internal sealed class Population<TChromosome> : IPopulation<TChromosome>
    where TChromosome : IChromosome
{
    private readonly TChromosome[] _chromosomes;
    private readonly double[] _fitness;

    public Population(IReadOnlyList<TChromosome> chromosomes, double[] fitness)
    {
        ArgumentNullException.ThrowIfNull(chromosomes);
        ArgumentNullException.ThrowIfNull(fitness);

        if (chromosomes.Count != fitness.Length)
        {
            throw new ArgumentException(
                $"Оценок {fitness.Length}, хромосом {chromosomes.Count}: массивы должны быть одной длины.",
                nameof(fitness));
        }

        _chromosomes = [.. chromosomes];
        _fitness = [.. fitness];
    }

    public int Count => _chromosomes.Length;

    public TChromosome this[int index] => _chromosomes[index];

    public double GetFitness(int index) => _fitness[index];

    public IEnumerator<TChromosome> GetEnumerator()
        => ((IEnumerable<TChromosome>)_chromosomes).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
