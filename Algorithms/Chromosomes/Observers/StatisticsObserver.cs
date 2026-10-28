using Core.Contracts;

namespace Algorithms.Observers;

/// <summary>
/// An observer that records summary statistics for each observed generation.
/// </summary>
/// <remarks>
/// This observer is intended for use from a single thread only. Its history and reusable evaluation
/// buffer are not synchronized.
/// </remarks>
/// <typeparam name="TChromosome">The chromosome type being observed.</typeparam>
public sealed class StatisticsObserver<TChromosome> : Core.Contracts.IObserver<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    private readonly ILossFunction<TChromosome> _lossFunction;
    private readonly List<GenerationStatistics> _history = [];
    private readonly IReadOnlyList<GenerationStatistics> _historyView;
    private double[] _lossBuffer = [];

    /// <summary>
    /// Creates a statistics observer.
    /// </summary>
    /// <param name="lossFunction">The loss function used to score individuals.</param>
    public StatisticsObserver(ILossFunction<TChromosome> lossFunction)
    {
        ArgumentNullException.ThrowIfNull(lossFunction);

        _lossFunction = lossFunction;
        _historyView = _history.AsReadOnly();
    }

    /// <summary>
    /// Gets the statistics recorded for each generation, in call order.
    /// </summary>
    public IReadOnlyList<GenerationStatistics> History => _historyView;

    /// <summary>
    /// Gets the statistics for the most recently observed generation, or <see langword="null"/> when
    /// no generation has been observed.
    /// </summary>
    public GenerationStatistics? Latest { get; private set; }

    /// <summary>
    /// Occurs after statistics for a generation have been recorded.
    /// </summary>
    public event Action<GenerationStatistics>? Updated;

    /// <summary>
    /// Evaluates the population and records its summary statistics.
    /// </summary>
    /// <param name="population">The current population.</param>
    public void OnGeneration(IReadOnlyList<TChromosome> population)
    {
        ArgumentNullException.ThrowIfNull(population);

        int count = population.Count;
        if (_lossBuffer.Length < count)
        {
            _lossBuffer = new double[count];
        }

        if (count == 0)
        {
            GenerationStatistics empty = new(_history.Count, 0d, 0d, 0d, 0d);
            _history.Add(empty);
            Latest = empty;
            Updated?.Invoke(empty);
            return;
        }

        double sum = 0d;
        double best = double.PositiveInfinity;
        double worst = double.NegativeInfinity;

        for (int i = 0; i < count; i++)
        {
            double loss = _lossFunction.Evaluate(population[i]);
            _lossBuffer[i] = loss;
            sum += loss;

            if (loss < best)
            {
                best = loss;
            }

            if (loss > worst)
            {
                worst = loss;
            }
        }

        double mean = sum / count;
        double squaredDifferenceSum = 0d;
        for (int i = 0; i < count; i++)
        {
            double difference = _lossBuffer[i] - mean;
            squaredDifferenceSum += difference * difference;
        }

        double stdDev = Math.Sqrt(squaredDifferenceSum / count);
        GenerationStatistics statistics = new(_history.Count, best, mean, worst, stdDev);
        _history.Add(statistics);
        Latest = statistics;
        Updated?.Invoke(statistics);
    }
}
