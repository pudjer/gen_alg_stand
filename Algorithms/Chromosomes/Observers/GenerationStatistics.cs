namespace Algorithms.Observers;

public sealed record GenerationStatistics(
    int Generation,
    double Best,
    double Mean,
    double Worst,
    double StdDev);
