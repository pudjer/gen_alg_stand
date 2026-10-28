using Core.Contracts;

namespace Algorithms.Observers;

public sealed class HistoryObserver<TChromosome> : Core.Contracts.IObserver<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    private readonly List<double> _bestLosses = [];
    private readonly List<double> _meanLosses = [];
    private readonly IReadOnlyList<double> _bestLossesView;
    private readonly IReadOnlyList<double> _meanLossesView;

    public HistoryObserver()
    {
        _bestLossesView = _bestLosses.AsReadOnly();
        _meanLossesView = _meanLosses.AsReadOnly();
    }

    public IReadOnlyList<double> BestLosses => _bestLossesView;

    public IReadOnlyList<double> MeanLosses => _meanLossesView;

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

        _bestLosses.Add(best);
        _meanLosses.Add(sum / population.Count);
    }
}
