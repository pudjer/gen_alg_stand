using Core.Contracts;

namespace Algorithms.Selections;

/// <summary>
/// Selects individuals only from the fittest fraction of the population.
/// </summary>
/// <typeparam name="TChromosome">The chromosome type being evolved.</typeparam>
public sealed class TruncationSelection<TChromosome> : ISelection<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    private readonly ILossFunction<TChromosome> _lossFunction;
    private readonly double _survivalRate;

    /// <summary>
    /// Initializes a new instance of the <see cref="TruncationSelection{TChromosome}"/> class.
    /// </summary>
    /// <param name="lossFunction">Fitness evaluator; lower values are better.</param>
    /// <param name="survivalRate">Fraction of the population kept as survivors; must be in (0, 1].</param>
    public TruncationSelection(ILossFunction<TChromosome> lossFunction, double survivalRate = 0.5)
    {
        ArgumentNullException.ThrowIfNull(lossFunction);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(survivalRate, 0.0, nameof(survivalRate));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(survivalRate, 1.0, nameof(survivalRate));

        _lossFunction = lossFunction;
        _survivalRate = survivalRate;
    }

    /// <inheritdoc />
    public IReadOnlyList<TChromosome> Select(IReadOnlyList<TChromosome> population, IRandom random)
    {
        ArgumentNullException.ThrowIfNull(population);
        ArgumentNullException.ThrowIfNull(random);

        int count = population.Count;
        if (count == 0)
        {
            return Array.Empty<TChromosome>();
        }

        TChromosome[] sorted = [.. population];
        double[] losses = new double[count];
        for (int i = 0; i < count; i++)
        {
            losses[i] = _lossFunction.Evaluate(sorted[i]);
        }

        Array.Sort(losses, sorted);

        int survivorCount = Math.Max(1, (int)Math.Ceiling(count * _survivalRate));
        if (survivorCount > count)
        {
            survivorCount = count;
        }

        List<TChromosome> result = new(count);
        for (int i = 0; i < count; i++)
        {
            result.Add(sorted[random.Next(0, survivorCount)]);
        }

        return result;
    }
}
