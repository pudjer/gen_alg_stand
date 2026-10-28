using System.Globalization;
using Catalog;
using Studio;

namespace Tests;

internal static class TestHelpers
{
    internal static ExperimentConfig WithAlgorithmParameter(ExperimentConfig config, string key, string value)
    {
        var parameters = new Dictionary<string, string>(config.AlgorithmParameters, StringComparer.Ordinal)
        {
            [key] = value,
        };

        return config with { AlgorithmParameters = parameters };
    }

    internal static ExperimentConfig WithBudget(ExperimentConfig config, int maxGenerations) =>
        WithAlgorithmParameter(config, "maxGenerations", maxGenerations.ToString(CultureInfo.InvariantCulture));

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