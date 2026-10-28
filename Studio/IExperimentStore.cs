namespace Studio;

public interface IExperimentStore
{
    Task<IReadOnlyList<StoredExperiment>> ListAsync(CancellationToken cancellationToken = default);

    Task<string> SaveAsync(ExperimentResult result, CancellationToken cancellationToken = default);

    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
