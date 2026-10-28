using System.Text.Json;
using Algorithms.Chromosomes;
using Core.Contracts;

namespace GenAlgStand.Web.Catalog.Visualizers;

public sealed class BinaryBitsVisualizer : IProblemVisualizer<BinaryChromosome>
{
    private const int MaxGenes = 5000;

    public Visualization? Build(VisualizationContext<BinaryChromosome> context)
    {
        int length = context.Best.Length;
        if (length is < 1 or > MaxGenes)
        {
            return null;
        }

        var bits = new int[length];
        int ones = 0;

        for (int index = 0; index < length; index++)
        {
            bits[index] = context.Best[index] ? 1 : 0;
            ones += bits[index];
        }

        string json = JsonSerializer.Serialize(new { bits, ones, count = length });

        return new Visualization(
            "bits",
            $"Биты лучшей особи · единиц: {ones} из {length}",
            json);
    }
}