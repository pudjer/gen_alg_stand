namespace Studio;

/// <summary>Runs an experiment and reports progress as generations complete.</summary>
public interface IExperimentRunner
{
    /// <summary>Runs the experiment described by <paramref name="config"/>.</summary>
    /// <param name="config">The configuration to run.</param>
    /// <param name="progress">Receives one update per generation; may be <see langword="null"/>.</param>
    /// <param name="cancellationToken">Cancels the run; a cancelled run returns a partial result.</param>
    /// <returns>The result, including the history collected so far.</returns>
    Task<ExperimentResult> RunAsync(
        ExperimentConfig config,
        IProgress<GenerationStats>? progress = null,
        CancellationToken cancellationToken = default);
}
