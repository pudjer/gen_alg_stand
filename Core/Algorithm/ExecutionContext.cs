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
        GeneticAlgorithmOptions options,
        IRandom random,
        CancellationToken cancellationToken)
    {
        Problem = problem;
        Options = options;
        Random = random;
        CancellationToken = cancellationToken;
        _history = [];
        Population = new Population<TChromosome>([], []);
    }

    public IGeneticProblem<TChromosome> Problem { get; }

    /// <summary>
    /// Настройки прогона. Не входят в <see cref="IExecutionContext{TChromosome}"/>:
    /// критерий останова не должен зависеть от вероятностей операторов.
    /// </summary>
    public GeneticAlgorithmOptions Options { get; }

    public int Generation { get; private set; }

    public IPopulation<TChromosome> Population { get; private set; }

    public TChromosome? Best { get; private set; }

    public double? BestLoss { get; private set; }

    public IReadOnlyList<IGenerationResult> History => _history;

    public IRandom Random { get; }

    public CancellationToken CancellationToken { get; }

    /// <summary>
    /// Продвигает контекст к следующей границе поколения. Оба значения —
    /// <see cref="Best"/> и <see cref="BestLoss"/> — обновляются одним
    /// вызовом, поэтому разъехаться они не могут.
    /// </summary>
    public void Advance(
        int generation,
        IPopulation<TChromosome> population,
        TChromosome best,
        double bestLoss)
    {
        Generation = generation;
        Population = population;
        Best = best;
        BestLoss = bestLoss;
    }

    /// <summary>Дописывает снимок поколения в историю.</summary>
    public void Record(IGenerationResult generation) => _history.Add(generation);
}