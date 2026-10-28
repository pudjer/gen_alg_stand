using System.Globalization;
using Core.Contracts;

namespace Algorithms.Observers;

public sealed class ConsoleObserver<TChromosome> : Core.Contracts.IObserver<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    private const string DefaultLabel = "GA";

    private readonly string _label;
    private int _generation;

    public ConsoleObserver(string? label = null)
    {
        _label = string.IsNullOrWhiteSpace(label) ? DefaultLabel : label;
    }

    public void OnGeneration(IReadOnlyList<Individ<TChromosome>> population)
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
            double loss = population[i].LossValue;
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
