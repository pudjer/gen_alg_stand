namespace Studio;

public sealed record GenerationStats(
    int Generation,
    double Best,
    double Mean,
    double Worst,
    double StdDev,
    double ElapsedMilliseconds);
