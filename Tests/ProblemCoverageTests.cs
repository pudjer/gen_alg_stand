using Catalog;
using Studio;

namespace Tests;

public sealed class ProblemCoverageTests
{
    private static readonly CatalogRegistry Catalog = new();

    public static TheoryData<string> AllProblems
    {
        get
        {
            var data = new TheoryData<string>();
            foreach (IProblemEntry problem in Catalog.Problems)
            {
                data.Add(problem.Key);
            }

            return data;
        }
    }

    [Theory]
    [MemberData(nameof(AllProblems))]
    public async Task EveryProblem_ReachesAFiniteBestLoss(string problemKey)
    {
        ExperimentResult result = await TestHelpers.RunAsync(problemKey, 10, seed: 3);

        Assert.True(result.Completed, $"{problemKey}: the run did not complete.");
        Assert.True(result.History.Count > 0, $"{problemKey}: the run produced no history.");
        Assert.True(double.IsFinite(result.BestLoss), $"{problemKey}: best loss was not finite ({result.BestLoss}).");
        Assert.True(
            double.IsFinite(result.TotalMilliseconds),
            $"{problemKey}: elapsed time was not finite ({result.TotalMilliseconds}).");
    }

    [Theory]
    [MemberData(nameof(AllProblems))]
    public async Task EveryProblem_ImprovesOrMatchesItsInitialBest(string problemKey)
    {
        ExperimentResult result = await TestHelpers.RunAsync(problemKey, 20, seed: 3);

        Assert.True(result.History.Count > 0, $"{problemKey}: the run produced no history.");
        Assert.True(
            result.History[0].Generation == 0,
            $"{problemKey}: history did not start at generation 0 (found {result.History[0].Generation}).");
        Assert.True(
            result.BestLoss <= result.History[0].Best + 1e-9,
            $"{problemKey}: best loss {result.BestLoss} is worse than the initial best {result.History[0].Best}.");
    }

    [Fact]
    public async Task LinearTsp_OptimumIsTwiceTheSpan()
    {
        const double optimum = 18.0;

        ExperimentResult result = await TestHelpers.RunAsync("problem.lineartsp", 200, seed: 1);

        Assert.True(result.Completed, "problem.lineartsp: the run did not complete.");
        Assert.True(
            double.IsFinite(result.BestLoss),
            $"problem.lineartsp: best loss was not finite ({result.BestLoss}).");
        Assert.True(
            result.BestLoss >= optimum - 1e-9,
            $"problem.lineartsp: best loss {result.BestLoss} is below the closed-tour lower bound {optimum}.");

        Assert.True(result.History.Count > 0, "problem.lineartsp: the run produced no history.");
        foreach (GenerationStats h in result.History)
        {
            Assert.True(
                h.Best >= optimum - 1e-9,
                $"problem.lineartsp: generation {h.Generation} best {h.Best} is below the lower bound {optimum}.");
        }
    }

    [Fact]
    public void NewProblems_AreRegisteredWithDistinctKeysAndNames()
    {
        Assert.Equal(20, Catalog.Problems.Count);

        var keys = new HashSet<string>(StringComparer.Ordinal);
        var names = new HashSet<string>(StringComparer.Ordinal);

        foreach (IProblemEntry problem in Catalog.Problems)
        {
            Assert.True(
                keys.Add(problem.Key),
                $"problem key '{problem.Key}' is registered more than once.");
            Assert.False(
                string.IsNullOrWhiteSpace(problem.DisplayName),
                $"problem '{problem.Key}' has a blank display name.");
            Assert.True(
                names.Add(problem.DisplayName),
                $"display name '{problem.DisplayName}' is used by more than one problem (last seen on '{problem.Key}').");
        }
    }

    [Fact]
    public void EveryProblem_ExposesAtLeastOneOperatorOfEveryFamily()
    {
        foreach (IProblemEntry problem in Catalog.Problems)
        {
            Assert.True(
                problem.Crossovers.Count > 0,
                $"problem '{problem.Key}' exposes no crossovers.");
            Assert.True(
                problem.Mutations.Count > 0,
                $"problem '{problem.Key}' exposes no mutations.");
            Assert.True(
                problem.Selections.Count > 0,
                $"problem '{problem.Key}' exposes no selections.");
            Assert.True(
                problem.InitialPopulations.Count > 0,
                $"problem '{problem.Key}' exposes no initial populations.");
            Assert.True(
                problem.Algorithms.Count > 0,
                $"problem '{problem.Key}' exposes no algorithms.");
        }
    }
}
