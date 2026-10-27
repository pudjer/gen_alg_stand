using System.Globalization;
using Core.Contracts;

namespace Algorithms.Observers;

/// <summary>
/// An observer that writes a one-line best/mean loss summary per generation to the console.
/// </summary>
/// <typeparam name="TChromosome">The chromosome type being observed.</typeparam>
public sealed class ConsoleObserver<TChromosome> : Core.Contracts.IObserver<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    private const string DefaultLabel = "GA";

    private readonly ILossFunction<TChromosome> _lossFunction;
    private readonly string _label;
    private int _generation;

    /// <summary>
    /// Creates a console observer.
    /// </summary>
    /// <param name="lossFunction">The loss function used to score individuals.</param>
    /// <param name="label">Optional label prefixed to each output line; defaults to <c>GA</c>.</param>
    public ConsoleObserver(ILossFunction<TChromosome> lossFunction, string? label = null)
    {
        ArgumentNullException.ThrowIfNull(lossFunction);

        _lossFunction = lossFunction;
        _label = string.IsNullOrWhiteSpace(label) ? DefaultLabel : label;
    }

    /// <summary>
    /// Computes the best and mean loss of the population and prints a single summary line.
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

        double mean = sum / population.Count;
        int generation = _generation++;

        string text = string.Format(
            CultureInfo.InvariantCulture,
            "[{0}] generation {1}: best={2:G6}, mean={3:G6}",
            _label,
            generation,
            best,
            mean);

        Console.WriteLine(text);
    }
}
