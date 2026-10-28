using Catalog;
using Studio;

namespace Tests;

public sealed class CancellationTests
{
    [Fact]
    public async Task LongRun_Cancelled_ReturnsIncompleteResultWithoutThrowing()
    {
        const int budget = 100_000;

        var catalog = new CatalogRegistry();
        IProblemEntry problem = catalog.FindProblem("problem.onemax")!;
        ExperimentConfig config = TestHelpers.WithBudget(
            catalog.CreateDefaultConfig("problem.onemax"),
            budget);

        using var cts = new CancellationTokenSource();
        Task<ExperimentResult> run = problem.CreateRunner(config).RunAsync(config, progress: null, cts.Token);

        await Task.Delay(50);
        await cts.CancelAsync();

        ExperimentResult result = await run;

        Assert.False(result.Completed);
        Assert.NotNull(result.History);
        Assert.True(result.History.Count < budget + 1, "The run finished before cancellation took effect.");
        Assert.Equal(result.Config, config);
    }

    [Fact]
    public async Task AlreadyCancelledToken_DoesNotThrow()
    {
        var catalog = new CatalogRegistry();
        IProblemEntry problem = catalog.FindProblem("problem.onemax")!;
        ExperimentConfig config = TestHelpers.WithBudget(
            catalog.CreateDefaultConfig("problem.onemax"),
            maxGenerations: 100_000);

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        ExperimentResult result = await problem.CreateRunner(config).RunAsync(config, progress: null, cts.Token);

        Assert.False(result.Completed);
    }
}