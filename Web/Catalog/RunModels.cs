using Parameters;

namespace GenAlgStand.Web.Catalog;

public sealed record ModuleOption(int Index, string Name, IReadOnlyList<IParameter> Parameters);

public sealed record RunRequest(
    int Loss,
    int Selection,
    int Crossover,
    int Mutation,
    int InitialPopulation,
    IReadOnlyDictionary<string, string?> ModuleParameters,
    int Generations,
    int EliteCount,
    int Seed,
    int MaxPoints,
    long MaxEvaluations,
    TimeSpan Budget);

public sealed record Sample(int Generation, double Average, double Best);

public sealed record RunResult(
    IReadOnlyList<Sample> Samples,
    double BestLoss,
    string BestGenome,
    int GenerationsRun,
    double ElapsedSeconds,
    bool Stopped,
    string Variant);

public interface IRunConfiguration
{
    IReadOnlyList<ModuleOption> LossFunctions { get; }

    IReadOnlyList<ModuleOption> Selections { get; }

    IReadOnlyList<ModuleOption> Crossovers { get; }

    IReadOnlyList<ModuleOption> Mutations { get; }

    IReadOnlyList<ModuleOption> InitialPopulations { get; }

    RunResult Run(RunRequest request);
}

public sealed class RunRejectedException(string message) : Exception(message);

public sealed class RunBudgetExceededException : Exception;