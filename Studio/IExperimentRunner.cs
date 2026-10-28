namespace Studio;

public interface IExperimentRunner
{
    Task<ExperimentResult> RunAsync(
        ExperimentConfig config,
        IProgress<GenerationStats>? progress = null,
        CancellationToken cancellationToken = default);
}
