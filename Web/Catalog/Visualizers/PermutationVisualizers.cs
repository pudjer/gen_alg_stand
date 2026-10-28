using System.Text.Json;
using Algorithms.Chromosomes;

namespace GenAlgStand.Web.Catalog.Visualizers;

public sealed class NQueensVisualizer : IProblemVisualizer<PermutationChromosome>
{
    private const int MaxSize = 60;

    public Visualization? Build(VisualizationContext<PermutationChromosome> context)
    {
        int size = context.Best.Length;
        if (size is < 1 or > MaxSize)
        {
            return null;
        }

        int[] queens = context.Best.Genes.ToArray();
        var conflicted = new bool[size];
        int conflicts = 0;

        for (int row = 0; row < size; row++)
        {
            for (int other = row + 1; other < size; other++)
            {
                if (Math.Abs(queens[row] - queens[other]) == other - row)
                {
                    conflicted[row] = true;
                    conflicted[other] = true;
                    conflicts++;
                }
            }
        }

        string json = JsonSerializer.Serialize(new { queens, conflicted });

        string title = conflicts == 0
            ? $"Доска N ферзей · решение без конфликтов ({size}×{size})"
            : $"Доска N ферзей · конфликтующих пар: {conflicts}";

        return new Visualization("nqueens", title, json);
    }
}