using Algorithms.Chromosomes;
using Parameters;

namespace Catalog.Problems;

internal static class ProblemTemplates
{
    internal static FloatChromosome FloatTemplate(ParameterSet parameters)
    {
        int size = parameters.GetInt("size");
        double min = parameters.GetDouble("minValue");
        double max = parameters.GetDouble("maxValue");

        if (min > max)
        {
            throw new ArgumentException("'minValue' must be less than or equal to 'maxValue'.");
        }

        double seed = Math.Clamp(0d, min, max);
        double[] genes = new double[size];
        Array.Fill(genes, seed);

        return new FloatChromosome(genes, min, max);
    }

    internal static IntChromosome IntTemplate(ParameterSet parameters)
    {
        int size = parameters.GetInt("size");
        int min = parameters.GetInt("minValue");
        int max = parameters.GetInt("maxValue");

        if (min > max)
        {
            throw new ArgumentException("'minValue' must be less than or equal to 'maxValue'.");
        }

        int seed = Math.Clamp(0, min, max);
        int[] genes = new int[size];
        Array.Fill(genes, seed);

        return new IntChromosome(genes, min, max);
    }
}
