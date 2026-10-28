using Algorithms.Chromosomes;
using Parameters;

namespace Catalog.Problems;

/// <summary>Shared helpers for building the template chromosome of a problem.</summary>
internal static class ProblemTemplates
{
    /// <summary>Builds a float template from the conventional <c>size</c>, <c>minValue</c>, <c>maxValue</c> parameters.</summary>
    internal static FloatChromosome FloatTemplate(ParameterSet parameters)
    {
        int size = parameters.GetInt("size");
        double min = parameters.GetDouble("minValue");
        double max = parameters.GetDouble("maxValue");

        if (min > max)
        {
            throw new ArgumentException("'minValue' must be less than or equal to 'maxValue'.");
        }

        // The genes only need to be valid placeholder values: chromosomes built from this template are
        // overwritten by the initial population generator, which redraws every gene inside [min, max].
        // Clamping the origin keeps the template valid even when the range excludes zero.
        double seed = Math.Clamp(0d, min, max);
        double[] genes = new double[size];
        Array.Fill(genes, seed);

        return new FloatChromosome(genes, min, max);
    }

    /// <summary>Builds an int template from the conventional <c>size</c>, <c>minValue</c>, <c>maxValue</c> parameters.</summary>
    internal static IntChromosome IntTemplate(ParameterSet parameters)
    {
        int size = parameters.GetInt("size");
        int min = parameters.GetInt("minValue");
        int max = parameters.GetInt("maxValue");

        if (min > max)
        {
            throw new ArgumentException("'minValue' must be less than or equal to 'maxValue'.");
        }

        // Same reasoning as FloatTemplate: the placeholder genes must be inside [min, max].
        int seed = Math.Clamp(0, min, max);
        int[] genes = new int[size];
        Array.Fill(genes, seed);

        return new IntChromosome(genes, min, max);
    }
}
