using System.Collections;
using Core.Contracts.Inner;
using Core.Contracts.Outer;

namespace Core.Algorithm;

/// <summary>
/// Популяция поколения: хромосомы и их потери хранятся параллельно,
/// поэтому обращение к loss — O(1) без словарей и упаковки.
/// <para>
/// Списки копируются при создании: после оценки движок владеет данными
/// безраздельно, и правка списка снаружи не разъедет оценки с хромосомами.
/// </para>
/// </summary>
internal sealed class Population<TChromosome> : IPopulation<TChromosome>
    where TChromosome : IChromosome
{
    private readonly TChromosome[] _chromosomes;
    private readonly double[] _loss;

    public Population(IReadOnlyList<TChromosome> chromosomes, double[] loss)
    {
        ArgumentNullException.ThrowIfNull(chromosomes);
        ArgumentNullException.ThrowIfNull(loss);

        if (chromosomes.Count != loss.Length)
        {
            throw new ArgumentException(
                $"Оценок {loss.Length}, хромосом {chromosomes.Count}: массивы должны быть одной длины.",
                nameof(loss));
        }

        _chromosomes = [.. chromosomes];
        _loss = [.. loss];
    }

    public int Count => _chromosomes.Length;

    public TChromosome this[int index] => _chromosomes[index];

    public double GetLoss(int index) => _loss[index];

    public IEnumerator<TChromosome> GetEnumerator()
        => ((IEnumerable<TChromosome>)_chromosomes).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
