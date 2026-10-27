using Core.Contracts;

namespace Algorithms.Observers;

/// <summary>
/// An observer that records the individual with the lowest loss seen across all generations.
/// </summary>
/// <typeparam name="TChromosome">The chromosome type being observed.</typeparam>
public sealed class BestChromosomeObserver<TChromosome> : Core.Contracts.IObserver<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    private readonly ILossFunction<TChromosome> _lossFunction;

    /// <summary>
    /// Creates a best-chromosome observer.
    /// </summary>
    /// <param name="lossFunction">The loss function used to score individuals.</param>
    public BestChromosomeObserver(ILossFunction<TChromosome> lossFunction)
    {
        ArgumentNullException.ThrowIfNull(lossFunction);

        _lossFunction = lossFunction;
    }

    /// <summary>
    /// Gets the best individual seen so far, or <see langword="null"/> when none has been seen.
    /// </summary>
    public TChromosome? Best { get; private set; }

    /// <summary>
    /// Gets the loss of the best individual seen so far; <see cref="double.PositiveInfinity"/> initially.
    /// </summary>
    public double BestLoss { get; private set; } = double.PositiveInfinity;

    /// <summary>
    /// Gets the number of <see cref="OnGeneration"/> calls received, including empty populations.
    /// </summary>
    public int Generation { get; private set; }

    /// <summary>
    /// Updates the tracked best individual from the supplied population.
    /// </summary>
    /// <param name="population">The current population.</param>
    public void OnGeneration(IReadOnlyList<TChromosome> population)
    {
        ArgumentNullException.ThrowIfNull(population);

        Generation++;

        for (int i = 0; i < population.Count; i++)
        {
            TChromosome candidate = population[i];
            double loss = _lossFunction.Evaluate(candidate);

            if (loss < BestLoss)
            {
                BestLoss = loss;
                Best = candidate;
            }
        }
    }
}
