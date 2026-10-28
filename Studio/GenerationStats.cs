namespace Studio;

/// <summary>Per-generation statistics collected during a run.</summary>
/// <param name="Generation">Zero-based generation index; generation 0 is the initial population.</param>
/// <param name="Best">Lowest loss in the population.</param>
/// <param name="Mean">Mean loss of the population.</param>
/// <param name="Worst">Highest loss in the population.</param>
/// <param name="StdDev">Population standard deviation of the loss.</param>
/// <param name="ElapsedMilliseconds">Wall clock time spent up to and including this generation.</param>
public sealed record GenerationStats(
    int Generation,
    double Best,
    double Mean,
    double Worst,
    double StdDev,
    double ElapsedMilliseconds);
