namespace Web.Components;

/// <summary>One line of a chart: a label, a CSS color and the y values to plot.</summary>
/// <param name="Label">Legend label.</param>
/// <param name="Color">CSS color used for the line and the legend swatch.</param>
/// <param name="Points">Values plotted at consecutive x positions, starting at x = 0.</param>
public sealed record ChartSeries(string Label, string Color, IReadOnlyList<double> Points);