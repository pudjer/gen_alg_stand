using Core.Contracts.Inner;
using Core.Contracts.Outer;

namespace Core.Algorithm;

/// <summary>
/// Контекст прогона — единственное, что главный цикл поставляет абстракциям.
/// Обновляется движком на каждой границе поколения; операторы видят
/// его только для чтения и не должны сохранять ссылку.
/// </summary>
internal sealed class ExecutionContext : IExecutionContext
{
    private readonly List<IGenerationResult> _history;

    public ExecutionContext(
        IFitnessFunction fitnessFunction,
        IRandom random,
        CancellationToken cancellationToken)
    {
        IsMaximization = fitnessFunction.IsMaximization;
        Random = random;
        CancellationToken = cancellationToken;
        _history = new List<IGenerationResult>();
        Population = [];
    }

    public int Generation { get; private set; }

    public bool IsMaximization { get; }

    public IReadOnlyList<Individual> Population { get; private set; }

    public Individual? Best { get; private set; }

    public IReadOnlyList<IGenerationResult> History => _history;

    public IRandom Random { get; }

    public CancellationToken CancellationToken { get; }

    /// <summary>Продвигает контекст к следующей границе поколения.</summary>
    public void Advance(int generation, IReadOnlyList<Individual> population, Individual? best)
    {
        Generation = generation;
        Population = population;
        Best = best;
    }

    /// <summary>Дописывает снимок поколения в историю.</summary>
    public void Record(IGenerationResult generation) => _history.Add(generation);
}
