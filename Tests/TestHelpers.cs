using System.Globalization;
using Catalog;
using Studio;

namespace Tests;

/// <summary>Shared helpers for building experiment configurations in tests.</summary>
internal static class TestHelpers
{
    /// <summary>
    /// Returns a copy of <paramref name="config"/> whose algorithm parameters carry the supplied
    /// generation budget.
    /// </summary>
    /// <param name="config">The configuration to copy.</param>
    /// <param name="key">Algorithm parameter key, for example <c>"maxGenerations"</c>.</param>
    /// <param name="value">The value to force.</param>
    /// <returns>A configuration with the replaced algorithm parameter.</returns>
    internal static ExperimentConfig WithAlgorithmParameter(ExperimentConfig config, string key, string value)
    {
        var parameters = new Dictionary<string, string>(config.AlgorithmParameters, StringComparer.Ordinal)
        {
            [key] = value,
        };

        return config with { AlgorithmParameters = parameters };
    }

    /// <summary>Forces the algorithm budget of a configuration.</summary>
    /// <param name="config">The configuration to copy.</param>
    /// <param name="maxGenerations">The generation budget.</param>
    /// <returns>A configuration with the forced budget.</returns>
    internal static ExperimentConfig WithBudget(ExperimentConfig config, int maxGenerations) =>
        WithAlgorithmParameter(config, "maxGenerations", maxGenerations.ToString(CultureInfo.InvariantCulture));

    /// <summary>
    /// Builds the default configuration of a problem, forces a budget and a seed, then runs it.
    /// </summary>
    /// <param name="problemKey">Problem key.</param>
    /// <param name="maxGenerations">Generation budget.</param>
    /// <param name="seed">Run seed.</param>
    /// <param name="algorithmKey">Optional algorithm key; defaults to the catalog default.</param>
    /// <returns>The run result.</returns>
    internal static async Task<ExperimentResult> RunAsync(
        string problemKey,
        int maxGenerations,
        int seed = 1,
        string? algorithmKey = null)
    {
        var catalog = new CatalogRegistry();
        ExperimentConfig config = catalog.CreateDefaultConfig(problemKey);

        if (algorithmKey is not null)
        {
            config = config with { AlgorithmKey = algorithmKey };
        }

        config = WithBudget(config with { Seed = seed }, maxGenerations);

        IProblemEntry problem = catalog.FindProblem(problemKey)
            ?? throw new InvalidOperationException($"Unknown problem '{problemKey}'.");

        return await problem.CreateRunner(config).RunAsync(config);
    }
}