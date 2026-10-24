using Core.Contracts.Inner;
using Core.Contracts.Outer;

namespace Core.Algorithm;

/// <summary>
/// Контекст прогона — единственное, что главный цикл поставляет абстракциям.
/// Обновляется движком на каждой границе поколения; операторы видят
/// его только для чтения и не должны сохранять ссылку.
/// </summary>
internal sealed class ExecutionContext<TChromosome> : IExecutionContext<TChromosome>
    where TChromosome : IChromosome
{
    private readonly List<IGenerationResult> _history;

    public ExecutionContext(
        IGeneticProblem<TChromosome> problem,
        IRandom random,
        CancellationToken cancellationToken)
    {
        Problem = problem;
        Random = random;
        CancellationToken = cancellationToken;
        _history = [];
        Population = new Population<TChromosome>([], []);
    }

    public IGeneticProblem<TChromosome> Problem { get; }

    public int Generation { get; private set; }

    public IPopulation<TChromosome> Population { get; private set; }

    public TChromosome? Best { get; private set; }

    public double? BestFitness { get; private set; }

    public IReadOnlyList<IGenerationResult> History => _history;

    public IRandom Random { get; }

    public CancellationToken CancellationToken { get; }

    /// <summary>
    /// Продвигает контекст к следующей границе поколения. Оба значения —
    /// <see cref="Best"/> и <see cref="BestFitness"/> — обновляются одним
    /// вызовом, поэтому разъехаться они не могут.
    /// </summary>
    public void Advance(
        int generation,
        IPopulation<TChromosome> population,
        TChromosome best,
        double bestFitness)
    {
        Generation = generation;
        Population = population;
        Best = best;
        BestFitness = bestFitness;
    }

    /// <summary>Дописывает снимок поколения в историю.</summary>
    public void Record(IGenerationResult generation) => _history.Add(generation);
}