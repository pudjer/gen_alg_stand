using Parameters;
using Studio;

namespace Catalog;

/// <summary>
/// A registry of everything the bench can run. The UI reads this to build its forms and uses
/// it to resolve a configuration into a runner.
/// </summary>
public interface ICatalog
{
    /// <summary>Gets all known problems.</summary>
    IReadOnlyList<IProblemEntry> Problems { get; }

    /// <summary>Finds a problem by key.</summary>
    /// <param name="key">The problem key.</param>
    /// <returns>The entry, or <see langword="null"/> when the key is unknown.</returns>
    IProblemEntry? FindProblem(string key);

    /// <summary>Builds a default configuration for a problem, using the defaults of every operator.</summary>
    /// <param name="problemKey">The problem key.</param>
    /// <returns>A configuration that is valid by construction.</returns>
    ExperimentConfig CreateDefaultConfig(string problemKey);
}
