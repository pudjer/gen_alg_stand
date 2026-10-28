using System.Text.Json;
using Algorithms.Chromosomes;
using Algorithms.LossFunctions;

namespace GenAlgStand.Web.Catalog.Visualizers;

public sealed class TspMapVisualizer : IProblemVisualizer<PermutationChromosome>
{
    private const int MaxCities = 300;

    public Visualization? Build(VisualizationContext<PermutationChromosome> context)
    {
        if (context.Loss is not TspLossFunction<PermutationChromosome> tsp)
        {
            return null;
        }

        IReadOnlyList<TspCity> cities = tsp.Cities;
        if (cities.Count != context.Best.Length || cities.Count is < 2 or > MaxCities)
        {
            return null;
        }

        var points = new double[cities.Count][];

        for (int index = 0; index < cities.Count; index++)
        {
            points[index] =
            [
                Math.Round(cities[index].X, 4),
                Math.Round(cities[index].Y, 4),
            ];
        }

        int[] order = context.Best.Genes.ToArray();
        string json = JsonSerializer.Serialize(new { cities = points, order });

        return new Visualization(
            "tsp-map",
            $"Карта городов и тур лучшей особи ({cities.Count} городов)",
            json);
    }
}