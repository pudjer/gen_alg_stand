using Catalog.Problems;
using Parameters;
using Studio;

namespace Catalog;

/// <summary>
/// The built-in catalog: every problem the bench knows about.
/// </summary>
public sealed class CatalogRegistry : ICatalog
{
    private static readonly IReadOnlyList<IProblemEntry> AllProblems =
    [
        new OneMaxProblem(),
        new SphereProblem(),
        new RastriginProblem(),
        new RosenbrockProblem(),
        new NQueensProblem(),
        new TspProblem(),
        new SumTargetProblem(),
        new TargetVectorProblem(),
    ];

    /// <inheritdoc />
    public IReadOnlyList<IProblemEntry> Problems => AllProblems;

    /// <inheritdoc />
    public IProblemEntry? FindProblem(string key)
    {
        ArgumentNullException.ThrowIfNull(key);

        for (int i = 0; i < AllProblems.Count; i++)
        {
            if (string.Equals(AllProblems[i].Key, key, StringComparison.Ordinal))
            {
                return AllProblems[i];
            }
        }

        return null;
    }

    /// <inheritdoc />
    public ExperimentConfig CreateDefaultConfig(string problemKey)
    {
        IProblemEntry problem = FindProblem(problemKey)
            ?? throw new ArgumentException($"Unknown problem '{problemKey}'.", nameof(problemKey));

        IOperatorDescriptor crossover = First(problem.Crossovers, problemKey);
        IOperatorDescriptor mutation = First(problem.Mutations, problemKey);
        IOperatorDescriptor selection = First(problem.Selections, problemKey);
        IOperatorDescriptor initialPopulation = First(problem.InitialPopulations, problemKey);
        IOperatorDescriptor algorithm = First(problem.Algorithms, problemKey);

        return new ExperimentConfig(
            problem.Key,
            Defaults(problem.Parameters),
            algorithm.Key,
            Defaults(algorithm.Parameters),
            crossover.Key,
            Defaults(crossover.Parameters),
            mutation.Key,
            Defaults(mutation.Parameters),
            selection.Key,
            Defaults(selection.Parameters),
            initialPopulation.Key,
            Defaults(initialPopulation.Parameters),
            Seed: 1);
    }

    private static IOperatorDescriptor First(IReadOnlyList<IOperatorDescriptor> descriptors, string problemKey) =>
        descriptors.Count > 0
            ? descriptors[0]
            : throw new InvalidOperationException($"Problem '{problemKey}' exposes no operators of a required family.");

    private static IReadOnlyDictionary<string, string> Defaults(IReadOnlyList<ParameterSpec> specs) =>
        ParameterSet.FromDefaults(specs).Values;
}
