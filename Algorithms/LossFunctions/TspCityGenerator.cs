using Core.Contracts;

namespace Algorithms.LossFunctions;

public enum TspLayout
{
    Random,
    Circle,
    Line,
}

public readonly record struct TspCity(double X, double Y);

public static class TspCityGenerator
{
    public const double Extent = 100d;

    public static IReadOnlyList<TspCity> Create(int count, int seed, TspLayout layout)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(count, 1);

        return layout switch
        {
            TspLayout.Random => CreateRandom(count, seed),
            TspLayout.Circle => CreateCircle(count, seed),
            TspLayout.Line => CreateLine(count, seed),
            _ => throw new ArgumentOutOfRangeException(nameof(layout), layout, "Unknown TSP layout."),
        };
    }

    public static double[,] CreateDistances(IReadOnlyList<TspCity> cities)
    {
        ArgumentNullException.ThrowIfNull(cities);

        if (cities.Count == 0)
        {
            throw new ArgumentException("At least one city is required.", nameof(cities));
        }

        var distances = new double[cities.Count, cities.Count];

        for (int i = 0; i < cities.Count; i++)
        {
            for (int j = i + 1; j < cities.Count; j++)
            {
                double dx = cities[i].X - cities[j].X;
                double dy = cities[i].Y - cities[j].Y;
                double distance = Math.Sqrt((dx * dx) + (dy * dy));

                distances[i, j] = distance;
                distances[j, i] = distance;
            }
        }

        return distances;
    }

    private static TspCity[] CreateRandom(int count, int seed)
    {
        var random = new SeededRandom(seed);
        var cities = new TspCity[count];

        for (int index = 0; index < count; index++)
        {
            cities[index] = new TspCity(
                random.NextDouble() * Extent,
                random.NextDouble() * Extent);
        }

        return cities;
    }

    private static TspCity[] CreateCircle(int count, int seed)
    {
        int[] slots = Shuffle(count, seed);
        var cities = new TspCity[count];
        const double center = Extent / 2d;
        const double radius = Extent / 2d;
        double step = count == 1 ? 0d : 2d * Math.PI / count;

        for (int index = 0; index < count; index++)
        {
            double angle = (-Math.PI / 2d) + (step * slots[index]);
            cities[index] = new TspCity(
                center + (radius * Math.Cos(angle)),
                center + (radius * Math.Sin(angle)));
        }

        return cities;
    }

    private static TspCity[] CreateLine(int count, int seed)
    {
        int[] slots = Shuffle(count, seed);
        var cities = new TspCity[count];
        const double line = Extent / 2d;

        for (int index = 0; index < count; index++)
        {
            double x = count == 1 ? line : Extent * slots[index] / (count - 1);
            cities[index] = new TspCity(x, line);
        }

        return cities;
    }

    private static int[] Shuffle(int count, int seed)
    {
        int[] slots = Enumerable.Range(0, count).ToArray();
        var random = new SeededRandom(seed);

        for (int index = count - 1; index > 0; index--)
        {
            int other = random.Next(0, index + 1);
            (slots[index], slots[other]) = (slots[other], slots[index]);
        }

        return slots;
    }
}