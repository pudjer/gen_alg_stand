namespace Studio;

/// <summary>
/// A complete, serializable description of one run: which problem, which operators, which
/// parameters and which seed. Two configs that compare equal must produce equal results.
/// </summary>
/// <param name="ProblemKey">Key of the problem entry, for example <c>"problem.onemax"</c>.</param>
/// <param name="ProblemParameters">Values for the problem's own parameters.</param>
/// <param name="AlgorithmKey">Key of the algorithm, for example <c>"algorithm.generational"</c>.</param>
/// <param name="AlgorithmParameters">Values for the algorithm's scheduling parameters.</param>
/// <param name="CrossoverKey">Key of the crossover operator.</param>
/// <param name="CrossoverParameters">Values for the crossover parameters.</param>
/// <param name="MutationKey">Key of the mutation operator.</param>
/// <param name="MutationParameters">Values for the mutation parameters.</param>
/// <param name="SelectionKey">Key of the selection operator.</param>
/// <param name="SelectionParameters">Values for the selection parameters.</param>
/// <param name="InitialPopulationKey">Key of the initial population generator.</param>
/// <param name="InitialPopulationParameters">Values for the initial population parameters.</param>
/// <param name="Seed">Seed for the random source; the same seed reproduces the run.</param>
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
