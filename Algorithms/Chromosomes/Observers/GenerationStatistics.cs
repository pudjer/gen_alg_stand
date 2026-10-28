namespace Algorithms.Observers;

/// <summary>
/// Summary statistics for one observed generation.
/// </summary>
/// <param name="Generation">The zero-based generation number.</param>
/// <param name="Best">The lowest loss in the population.</param>
/// <param name="Mean">The arithmetic mean loss in the population.</param>
/// <param name="Worst">The highest loss in the population.</param>
/// <param name="StdDev">The population standard deviation of the losses.</param>
public sealed record GenerationStatistics(
    int Generation,
    double Best,
    double Mean,
    double Worst,
    double StdDev);
