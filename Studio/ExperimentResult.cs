namespace Studio;

/// <summary>The outcome of one run.</summary>
/// <param name="Config">The configuration that produced this result.</param>
/// <param name="History">Per-generation statistics, starting at generation 0.</param>
/// <param name="BestLoss">The best loss reached.</param>
/// <param name="TotalMilliseconds">Total wall clock time of the run.</param>
/// <param name="Completed">Whether the run finished or was cancelled.</param>
public sealed record ExperimentResult(
    ExperimentConfig Config,
    IReadOnlyList<GenerationStats> History,
    double BestLoss,
    double TotalMilliseconds,
    bool Completed);
