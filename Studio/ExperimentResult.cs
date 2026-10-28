namespace Studio;

public sealed record ExperimentResult(
    ExperimentConfig Config,
    IReadOnlyList<GenerationStats> History,
    double BestLoss,
    double TotalMilliseconds,
    bool Completed);
