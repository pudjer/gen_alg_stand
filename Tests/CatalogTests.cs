using System.Text.RegularExpressions;
using Catalog;
using Parameters;
using Studio;

namespace Tests;

public sealed class CatalogTests
{
    private static readonly CatalogRegistry Catalog = new();

    [Fact]
    public void EveryProblem_IsWellFormed()
    {
        Assert.NotEmpty(Catalog.Problems);

        foreach (IProblemEntry problem in Catalog.Problems)
        {
            Assert.False(string.IsNullOrWhiteSpace(problem.Key));
            Assert.StartsWith("problem.", problem.Key, StringComparison.Ordinal);
            Assert.False(string.IsNullOrWhiteSpace(problem.DisplayName));
            Assert.NotEmpty(problem.Crossovers);
            Assert.NotEmpty(problem.Mutations);
            Assert.NotEmpty(problem.Selections);
            Assert.NotEmpty(problem.InitialPopulations);
            Assert.NotEmpty(problem.Algorithms);
        }
    }

    [Fact]
    public void ProblemKeys_AreUnique()
    {
        string[] keys = [.. Catalog.Problems.Select(p => p.Key)];
        string[] distinct = [.. keys.Distinct(StringComparer.Ordinal)];

        Assert.Equal(distinct.Length, keys.Length);
    }

    [Fact]
    public void CreateDefaultConfig_WorksForEveryProblem()
    {
        foreach (IProblemEntry problem in Catalog.Problems)
        {
            ExperimentConfig config = Catalog.CreateDefaultConfig(problem.Key);

            Assert.Equal(problem.Key, config.ProblemKey);
            Assert.False(string.IsNullOrWhiteSpace(config.AlgorithmKey));
            Assert.False(string.IsNullOrWhiteSpace(config.CrossoverKey));
            Assert.False(string.IsNullOrWhiteSpace(config.MutationKey));
            Assert.False(string.IsNullOrWhiteSpace(config.SelectionKey));
            Assert.False(string.IsNullOrWhiteSpace(config.InitialPopulationKey));
        }
    }

    [Fact]
    public void CreateDefaultConfig_OnlyNamesOperatorsOfTheProblem()
    {
        foreach (IProblemEntry problem in Catalog.Problems)
        {
            ExperimentConfig config = Catalog.CreateDefaultConfig(problem.Key);

            Assert.Contains(config.CrossoverKey, Keys(problem.Crossovers));
            Assert.Contains(config.MutationKey, Keys(problem.Mutations));
            Assert.Contains(config.SelectionKey, Keys(problem.Selections));
            Assert.Contains(config.InitialPopulationKey, Keys(problem.InitialPopulations));
            Assert.Contains(config.AlgorithmKey, Keys(problem.Algorithms));
        }
    }

    [Fact]
    public void CreateDefaultConfig_ProducesAcceptedValues()
    {
        foreach (IProblemEntry problem in Catalog.Problems)
        {
            ExperimentConfig config = Catalog.CreateDefaultConfig(problem.Key);

            ParameterSet.Create(problem.Parameters, config.ProblemParameters);
            ParameterSet.Create(Descriptor(problem.Crossovers, config.CrossoverKey).Parameters, config.CrossoverParameters);
            ParameterSet.Create(Descriptor(problem.Mutations, config.MutationKey).Parameters, config.MutationParameters);
            ParameterSet.Create(Descriptor(problem.Selections, config.SelectionKey).Parameters, config.SelectionParameters);
            ParameterSet.Create(
                Descriptor(problem.InitialPopulations, config.InitialPopulationKey).Parameters,
                config.InitialPopulationParameters);
            ParameterSet.Create(Descriptor(problem.Algorithms, config.AlgorithmKey).Parameters, config.AlgorithmParameters);
        }
    }

    [Fact]
    public void DescriptorKeys_AreUniqueWithinEachFamily()
    {
        foreach (IProblemEntry problem in Catalog.Problems)
        {
            AssertUnique(problem, "crossover", problem.Crossovers);
            AssertUnique(problem, "mutation", problem.Mutations);
            AssertUnique(problem, "selection", problem.Selections);
            AssertUnique(problem, "population", problem.InitialPopulations);
            AssertUnique(problem, "algorithm", problem.Algorithms);
        }
    }

    [Fact]
    public void DescriptorKeys_FollowTheLowercaseConvention()
    {
        var pattern = new Regex("^[a-z]+\\.[a-z0-9]+$", RegexOptions.CultureInvariant);

        foreach (IProblemEntry problem in Catalog.Problems)
        {
            AssertConvention(problem, "crossover", problem.Crossovers, pattern);
            AssertConvention(problem, "mutation", problem.Mutations, pattern);
            AssertConvention(problem, "selection", problem.Selections, pattern);
            AssertConvention(problem, "population", problem.InitialPopulations, pattern);
            AssertConvention(problem, "algorithm", problem.Algorithms, pattern);
        }
    }

    [Fact]
    public void DescriptorParameters_AreUniqueAndTheirDefaultsAreAccepted()
    {
        foreach (IProblemEntry problem in Catalog.Problems)
        {
            AssertParameters(problem.Key, problem.Parameters);
            AssertParameters(problem.Key, problem.Crossovers);
            AssertParameters(problem.Key, problem.Mutations);
            AssertParameters(problem.Key, problem.Selections);
            AssertParameters(problem.Key, problem.InitialPopulations);
            AssertParameters(problem.Key, problem.Algorithms);
        }
    }

    private static void AssertUnique(IProblemEntry problem, string family, IReadOnlyList<IOperatorDescriptor> descriptors)
    {
        string[] keys = [.. descriptors.Select(d => d.Key)];

        Assert.True(
            keys.Length == keys.Distinct(StringComparer.Ordinal).Count(),
            $"Problem '{problem.Key}' has duplicate {family} keys: {string.Join(", ", keys)}.");
    }

    private static void AssertConvention(
        IProblemEntry problem,
        string family,
        IReadOnlyList<IOperatorDescriptor> descriptors,
        Regex pattern)
    {
        foreach (IOperatorDescriptor descriptor in descriptors)
        {
            Assert.True(
                pattern.IsMatch(descriptor.Key),
                $"Problem '{problem.Key}': {family} key '{descriptor.Key}' is not '<category>.<name>' lowercase.");
            Assert.StartsWith($"{family}.", descriptor.Key, StringComparison.Ordinal);
            Assert.False(string.IsNullOrWhiteSpace(descriptor.DisplayName));
        }
    }

    private static void AssertParameters(string problemKey, IReadOnlyList<ParameterSpec> specs)
    {
        string[] keys = [.. specs.Select(s => s.Key)];
        Assert.True(
            keys.Length == keys.Distinct(StringComparer.Ordinal).Count(),
            $"Problem '{problemKey}' has duplicate parameter keys.");

        ParameterSet defaults = ParameterSet.FromDefaults(specs);
        ParameterSet.Create(specs, defaults.Values);
    }

    private static void AssertParameters(string problemKey, IReadOnlyList<IOperatorDescriptor> descriptors)
    {
        foreach (IOperatorDescriptor descriptor in descriptors)
        {
            AssertParameters($"{problemKey}/{descriptor.Key}", descriptor.Parameters);
        }
    }

    private static IReadOnlyList<string> Keys(IReadOnlyList<IOperatorDescriptor> descriptors) =>
        [.. descriptors.Select(d => d.Key)];

    private static IOperatorDescriptor Descriptor(IReadOnlyList<IOperatorDescriptor> descriptors, string key) =>
        descriptors.FirstOrDefault(d => string.Equals(d.Key, key, StringComparison.Ordinal))
        ?? throw new InvalidOperationException($"Descriptor '{key}' was not found.");
}