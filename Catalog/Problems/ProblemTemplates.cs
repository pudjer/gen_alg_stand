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

        return new FloatChromosome(new double[size], min, max);
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

        return new IntChromosome(new int[size], min, max);
    }
}
