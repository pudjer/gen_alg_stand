using Catalog;
using Studio;

namespace Tests;

/// <summary>
/// Determinism is the core promise of the bench: the same configuration and seed must reproduce the
/// same history, generation by generation.
/// </summary>
public sealed class ExperimentDeterminismTests
{
    public static TheoryData<string> RepresentativeProblems => new()
    {
        "problem.onemax",
        "problem.longestrun",
        "problem.sphere",
        "problem.ackley",
        "problem.bukin",
        "problem.nqueens",
        "problem.inversions",
        "problem.lineartsp",
        "problem.sumtarget",
    };

    private const int Budget = 20;

    [Theory]
    [MemberData(nameof(RepresentativeProblems))]
    public async Task SameConfigAndSeed_ProducesIdenticalHistory(string problemKey)
    {
        ExperimentResult first = await TestHelpers.RunAsync(problemKey, Budget, seed: 1);
        ExperimentResult second = await TestHelpers.RunAsync(problemKey, Budget, seed: 1);

        Assert.True(first.Completed, $"{problemKey}: the first run did not complete.");
        Assert.True(second.Completed, $"{problemKey}: the second run did not complete.");
        Assert.Equal(first.History.Count, second.History.Count);

        for (int i = 0; i < first.History.Count; i++)
        {
            GenerationStats expected = first.History[i];
            GenerationStats actual = second.History[i];

            Assert.Equal(expected.Generation, actual.Generation);
            Assert.Equal(expected.Best, actual.Best);
            Assert.Equal(expected.Mean, actual.Mean);
            Assert.Equal(expected.Worst, actual.Worst);
            Assert.Equal(expected.StdDev, actual.StdDev);
        }
    }

    [Theory]
    [MemberData(nameof(RepresentativeProblems))]
    public async Task History_StartsWithGenerationZero_AndHasBudgetPlusOneEntries(string problemKey)
    {
        ExperimentResult result = await TestHelpers.RunAsync(problemKey, Budget, seed: 1);

        Assert.Equal(Budget + 1, result.History.Count);
        Assert.Equal(0, result.History[0].Generation);

        for (int i = 0; i < result.History.Count; i++)
        {
            Assert.Equal(i, result.History[i].Generation);
        }
    }

    [Theory]
    [MemberData(nameof(RepresentativeProblems))]
    public async Task DifferentSeed_ChangesTheHistory(string problemKey)
    {
        ExperimentResult first = await TestHelpers.RunAsync(problemKey, Budget, seed: 1);
        ExperimentResult second = await TestHelpers.RunAsync(problemKey, Budget, seed: 987654);

        Assert.Equal(first.History.Count, second.History.Count);

        bool differs = false;
        for (int i = 0; i < first.History.Count; i++)
        {
            if (first.History[i].Best != second.History[i].Best
                || first.History[i].Mean != second.History[i].Mean
                || first.History[i].Worst != second.History[i].Worst
                || first.History[i].StdDev != second.History[i].StdDev)
            {
                differs = true;
                break;
            }
        }

        Assert.True(differs, $"{problemKey}: changing the seed did not change the history.");
    }

    [Fact]
    public async Task BestLoss_IsFiniteAndMatchesTheBestObservedGeneration()
    {
        ExperimentResult result = await TestHelpers.RunAsync("problem.onemax", Budget, seed: 1);

        Assert.True(double.IsFinite(result.BestLoss));
        Assert.Equal(result.History.Min(h => h.Best), result.BestLoss);
    }
}