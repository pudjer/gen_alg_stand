namespace Studio;

/// <summary>A stored experiment: the result plus the identifier used to delete it.</summary>
/// <param name="Id">Stable identifier assigned when the result was saved.</param>
/// <param name="Result">The stored result.</param>
public sealed record StoredExperiment(string Id, ExperimentResult Result);
