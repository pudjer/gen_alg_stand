using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.LossFunctions;

/// <summary>
/// Minimization loss computing the closed-tour length of a permutation over a distance matrix.
/// </summary>
/// <typeparam name="TChromosome">The permutation chromosome type being evaluated.</typeparam>
public sealed class TspLossFunction<TChromosome> : ILossFunction<TChromosome>
    where TChromosome : IPermutationChromosome<TChromosome>
{
    private readonly double[,] _distances;

    /// <summary>
    /// Initializes a new instance from a square matrix of finite, non-negative distances.
    /// </summary>
    /// <param name="distanceMatrix">A square matrix holding the pairwise city distances.</param>
    /// <exception cref="ArgumentNullException">The supplied matrix is null.</exception>
    /// <exception cref="ArgumentException">The matrix is not square or holds negative/invalid distances.</exception>
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
    }

    /// <summary>
    /// Returns the sum of consecutive edge distances plus the closing edge back to the first city.
    /// </summary>
    /// <param name="chromosome">The permutation chromosome describing the tour order.</param>
    /// <returns>The total cycle cost, or zero when the tour has fewer than two cities.</returns>
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
