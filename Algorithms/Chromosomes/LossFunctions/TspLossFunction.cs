using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.LossFunctions;

public sealed class TspLossFunction<TChromosome> : ILossFunction<TChromosome>
    where TChromosome : IPermutationChromosome<TChromosome>
{
    private readonly double[,] _distances;

    public TspLossFunction(double[,] distanceMatrix)
    {
        ArgumentNullException.ThrowIfNull(distanceMatrix);

        int dimension = distanceMatrix.GetLength(0);
        if (dimension != distanceMatrix.GetLength(1))
        {
            throw new ArgumentException("The distance matrix must be square.", nameof(distanceMatrix));
        }

        for (int i = 0; i < dimension; i++)
        {
            for (int j = 0; j < dimension; j++)
            {
                double distance = distanceMatrix[i, j];
                if (!double.IsFinite(distance) || distance < 0d)
                {
                    throw new ArgumentException(
                        "The distance matrix must contain finite, non-negative distances; a negative or invalid distance was found.",
                        nameof(distanceMatrix));
                }
            }
        }

        _distances = (double[,])distanceMatrix.Clone();
        Cities = [];
    }

    public TspLossFunction(IReadOnlyList<TspCity> cities)
    {
        ArgumentNullException.ThrowIfNull(cities);

        if (cities.Count == 0)
        {
            throw new ArgumentException("At least one city is required.", nameof(cities));
        }

        Cities = [.. cities];
        _distances = TspCityGenerator.CreateDistances(Cities);
    }

    public IReadOnlyList<TspCity> Cities { get; }

    public double Evaluate(TChromosome chromosome)
    {
        ArgumentNullException.ThrowIfNull(chromosome);

        int length = chromosome.Length;
        if (length < 2)
        {
            return 0d;
        }

        double[,] distances = _distances;
        if (distances.GetLength(0) != length)
        {
            throw new ArgumentException(
                $"The distance matrix dimension ({distances.GetLength(0)}) does not match the chromosome length ({length}).",
                nameof(chromosome));
        }

        for (int i = 0; i < length; i++)
        {
            int gene = chromosome[i];
            if (gene < 0 || gene >= length)
            {
                throw new ArgumentException(
                    $"Gene value {gene} is outside the valid range [0, {length}).",
                    nameof(chromosome));
            }
        }

        int first = chromosome[0];
        int previous = first;
        double total = 0d;

        for (int i = 1; i < length; i++)
        {
            int current = chromosome[i];
            total += distances[previous, current];
            previous = current;
        }

        total += distances[previous, first];
        return total;
    }
}
