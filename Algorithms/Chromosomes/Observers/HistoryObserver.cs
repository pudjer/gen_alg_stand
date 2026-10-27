using Core.Contracts;

namespace Algorithms.Observers;

/// <summary>
/// An observer that records the best and mean loss of each non-empty generation.
/// </summary>
/// <typeparam name="TChromosome">The chromosome type being observed.</typeparam>
public sealed class HistoryObserver<TChromosome> : Core.Contracts.IObserver<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    private readonly ILossFunction<TChromosome> _lossFunction;
    private readonly List<double> _bestLosses = [];
    private readonly List<double> _meanLosses = [];
    private readonly IReadOnlyList<double> _bestLossesView;
    private readonly IReadOnlyList<double> _meanLossesView;

    /// <summary>
    /// Creates a history observer.
    /// </summary>
    /// <param name="lossFunction">The loss function used to score individuals.</param>
    public HistoryObserver(ILossFunction<TChromosome> lossFunction)
    {
        ArgumentNullException.ThrowIfNull(lossFunction);

        _lossFunction = lossFunction;
        _bestLossesView = _bestLosses.AsReadOnly();
        _meanLossesView = _meanLosses.AsReadOnly();
    }

    /// <summary>
    /// Gets the recorded best loss per non-empty generation, in call order.
    /// </summary>
    public IReadOnlyList<double> BestLosses => _bestLossesView;

    /// <summary>
    /// Gets the recorded mean loss per non-empty generation, in call order.
    /// </summary>
    public IReadOnlyList<double> MeanLosses => _meanLossesView;

    /// <summary>
    /// Appends the best and mean loss of the population to the history.
    /// </summary>
    /// <param name="population">The current population.</param>
    public void OnGeneration(IReadOnlyList<TChromosome> population)
    {
        ArgumentNullException.ThrowIfNull(population);

        if (population.Count == 0)
        {
            return;
        }

        double best = double.PositiveInfinity;
        double sum = 0d;

        for (int i = 0; i < population.Count; i++)
        {
            double loss = _lossFunction.Evaluate(population[i]);
            sum += loss;

            if (loss < best)
            {
                best = loss;
            }
        }

        _bestLosses.Add(best);
        _meanLosses.Add(sum / population.Count);
    }
}
