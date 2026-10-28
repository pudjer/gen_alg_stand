using System.Text.Json;
using Core.Contracts;

namespace GenAlgStand.Web.Catalog.Visualizers;

public sealed class NumericVisualizer<TChromosome> : IProblemVisualizer<TChromosome>
    where TChromosome : class, IChromosome<TChromosome>
{
    private const int MaxGridEvaluations = 2_000_000;
    private const int MaxSide = 72;
    private const int CurvePoints = 240;

    private readonly Func<TChromosome, IReadOnlyList<double>> _genes;
    private readonly Func<TChromosome, (double Min, double Max)> _bounds;
    private readonly Func<double[], TChromosome> _create;
    private readonly bool _integer;

    public NumericVisualizer(
        Func<TChromosome, IReadOnlyList<double>> genes,
        Func<TChromosome, (double Min, double Max)> bounds,
        Func<double[], TChromosome> create,
        bool integer)
    {
        _genes = genes;
        _bounds = bounds;
        _create = create;
        _integer = integer;
    }

    public Visualization? Build(VisualizationContext<TChromosome> context)
    {
        IReadOnlyList<double> genes = _genes(context.Best);
        if (genes.Count == 0)
        {
            return null;
        }

        (double min, double max) = _bounds(context.Best);
        if (!double.IsFinite(min) || !double.IsFinite(max) || max - min < 1e-12)
        {
            return null;
        }

        return genes.Count == 1
            ? BuildCurve(context, genes, min, max)
            : BuildSlice(context, genes, min, max);
    }

    private Visualization BuildCurve(
        VisualizationContext<TChromosome> context,
        IReadOnlyList<double> genes,
        double min,
        double max)
    {
        int count = _integer
            ? (int)Math.Min(CurvePoints, Math.Max(2d, max - min + 1d))
            : CurvePoints;

        var xs = new double[count];
        var ys = new double[count];
        double[] probe = genes.ToArray();

        for (int index = 0; index < count; index++)
        {
            double x = Sample(min, max, index, count);
            probe[0] = x;
            xs[index] = x;
            ys[index] = Safe(context.Loss.Evaluate(_create(probe)));
        }

        double bestY = Safe(context.Loss.Evaluate(context.Best));
        string json = JsonSerializer.Serialize(new
        {
            xs,
            ys,
            best = new[] { genes[0], bestY },
        });

        return new Visualization("curve", "Кривая функции потерь по гену", json);
    }

    private Visualization? BuildSlice(
        VisualizationContext<TChromosome> context,
        IReadOnlyList<double> genes,
        double min,
        double max)
    {
        int length = genes.Count;
        int allowedSide = (int)Math.Floor(Math.Sqrt(MaxGridEvaluations / (double)length));
        if (allowedSide < 4)
        {
            return null;
        }

        int sideLimit = Math.Min(MaxSide, allowedSide);
        int nx = sideLimit;
        int ny = sideLimit;

        if (_integer)
        {
            long range = (long)Math.Round(max - min) + 1;
            nx = (int)Math.Min(sideLimit, Math.Max(2L, range));
            ny = nx;
        }

        double[] probe = genes.ToArray();
        var values = new double[nx * ny];

        for (int row = 0; row < ny; row++)
        {
            double y = Sample(min, max, ny - 1 - row, ny);

            for (int column = 0; column < nx; column++)
            {
                double x = Sample(min, max, column, nx);
                probe[0] = x;
                probe[1] = y;
                values[(row * nx) + column] = Safe(context.Loss.Evaluate(_create(probe)));
            }
        }

        string json = JsonSerializer.Serialize(new
        {
            x0 = min,
            x1 = max,
            y0 = min,
            y1 = max,
            nx,
            ny,
            values,
            best = new[] { genes[0], genes[1] },
        });

        string title = length == 2
            ? "Поверхность функции потерь"
            : "Срез функции потерь по первым двум генам";

        return new Visualization("heatmap", title, json);
    }

    private double Sample(double min, double max, int index, int count)
    {
        if (count <= 1)
        {
            return min;
        }

        double value = min + ((max - min) * index / (count - 1));
        return _integer ? Math.Round(value, MidpointRounding.AwayFromZero) : value;
    }

    private static double Safe(double value) => double.IsFinite(value) ? Math.Round(value, 6) : 0d;
}