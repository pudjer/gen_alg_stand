using Catalog;
using Studio;

namespace Tests;

public sealed class IslandObserverTests
{
    private const string ProblemKey = "problem.onemax";
    private const string IslandAlgorithmKey = "algorithm.island";
    private const int Budget = 20;

    [Fact]
    public async Task IslandRun_ReportsGenerationZeroAndBudgetPlusOneEntries()
    {
        ExperimentResult result = await TestHelpers.RunAsync(
            ProblemKey,
            Budget,
            seed: 1,
            algorithmKey: IslandAlgorithmKey);

        Assert.Equal(IslandAlgorithmKey, result.Config.AlgorithmKey);
        Assert.Equal(Budget + 1, result.History.Count);
        Assert.Equal(0, result.History[0].Generation);

        for (int i = 0; i < result.History.Count; i++)
        {
            Assert.Equal(i, result.History[i].Generation);
        }
    }

    [Fact]
    public async Task IslandRun_CompletesWithFiniteBestLoss()
    {
        ExperimentResult result = await TestHelpers.RunAsync(
            ProblemKey,
            Budget,
            seed: 1,
            algorithmKey: IslandAlgorithmKey);

        Assert.True(result.Completed);
        Assert.True(double.IsFinite(result.BestLoss), "The island run produced a non-finite best loss.");
        Assert.NotEmpty(result.History);
    }

    [Fact]
    public async Task IslandRun_IsDeterministicForTheSameSeed()
    {
        ExperimentResult first = await TestHelpers.RunAsync(
            ProblemKey,
            Budget,
            seed: 1,
            algorithmKey: IslandAlgorithmKey);
        ExperimentResult second = await TestHelpers.RunAsync(
            ProblemKey,
            Budget,
            seed: 1,
            algorithmKey: IslandAlgorithmKey);

        Assert.Equal(first.History.Count, second.History.Count);

        for (int i = 0; i < first.History.Count; i++)
        {
            Assert.Equal(first.History[i].Best, second.History[i].Best);
            Assert.Equal(first.History[i].Mean, second.History[i].Mean);
            Assert.Equal(first.History[i].Worst, second.History[i].Worst);
            Assert.Equal(first.History[i].StdDev, second.History[i].StdDev);
        }
    }
}