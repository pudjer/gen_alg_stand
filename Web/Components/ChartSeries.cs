namespace Web.Components;

public sealed record ChartSeries(string Label, string Color, IReadOnlyList<double> Points);