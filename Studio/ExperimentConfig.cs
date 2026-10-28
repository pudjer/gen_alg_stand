namespace Studio;

public sealed record ExperimentConfig(
    string ProblemKey,
    IReadOnlyDictionary<string, string> ProblemParameters,
    string AlgorithmKey,
    IReadOnlyDictionary<string, string> AlgorithmParameters,
    string CrossoverKey,
    IReadOnlyDictionary<string, string> CrossoverParameters,
    string MutationKey,
    IReadOnlyDictionary<string, string> MutationParameters,
    string SelectionKey,
    IReadOnlyDictionary<string, string> SelectionParameters,
    string InitialPopulationKey,
    IReadOnlyDictionary<string, string> InitialPopulationParameters,
    int Seed);
