namespace Studio;

/// <summary>Persists experiment results so they can be listed and compared later.</summary>
public interface IExperimentStore
{
    /// <summary>Returns all stored results, newest first.</summary>
    /// <param name="cancellationToken">Cancels the operation.</param>
    /// <returns>The stored results, each carrying the identifier needed to delete it.</returns>
    Task<IReadOnlyList<StoredExperiment>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>Saves a result and returns its assigned identifier.</summary>
    /// <param name="result">The result to save.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    /// <returns>The identifier assigned to the saved result.</returns>
    Task<string> SaveAsync(ExperimentResult result, CancellationToken cancellationToken = default);

    /// <summary>Removes a stored result.</summary>
    /// <param name="id">Identifier returned by <see cref="ListAsync"/> ordering.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
