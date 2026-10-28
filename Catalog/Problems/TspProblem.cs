using Algorithms.Chromosomes;
using Algorithms.LossFunctions;
using Catalog.Descriptors;
using Core.Contracts;
using Parameters;

namespace Catalog.Problems;

/// <summary>
/// Travelling salesman: minimize the closed-tour length over a permutation.
/// </summary>
/// <remarks>
/// A distance matrix is not a scalar parameter, so the problem generates one deterministically from
/// <c>matrixSeed</c>: city positions are drawn uniformly from the unit square with a
/// <see cref="Algorithms.Random.SeededRandom"/>, and distances are the Euclidean distances between
/// them. The same seed always yields the same matrix, which keeps runs reproducible.
/// </remarks>
public sealed class TspProblem : ProblemEntry<PermutationChromosome>
{
    private static readonly IReadOnlyList<ParameterSpec> Specs =
    [
        new("size", "Number of cities", ParameterKind.Int, "10", Min: 2, Max: 500, Step: 1),
        new("matrixSeed", "Distance matrix seed", ParameterKind.Int, "7", Min: int.MinValue, Max: int.MaxValue, Step: 1),
    ];

    /// <inheritdoc />
    public override string Key => "problem.tsp";

    /// <inheritdoc />
    public override string DisplayName => "Travelling salesman";

    /// <inheritdoc />
    public override IReadOnlyList<ParameterSpec> Parameters => Specs;

    /// <inheritdoc />
    protected override ProblemOperators<PermutationChromosome> BuildOperators(ParameterSet problemParameters)
    {
        int size = problemParameters.GetInt("size");
        int seed = problemParameters.GetInt("matrixSeed");

        PermutationChromosome template = PermutationChromosome.Identity(size);
        ILossFunction<PermutationChromosome> loss = new TspLossFunction<PermutationChromosome>(CreateDistanceMatrix(size, seed));

        return new ProblemOperators<PermutationChromosome>
        {
            LossFunction = loss,
            Crossovers = CrossoverDescriptors.ForOrdered<PermutationChromosome>(),
            Mutations = MutationDescriptors.ForOrdered<PermutationChromosome>(),
            Selections = SelectionDescriptors.For(loss),
            InitialPopulations = InitialPopulationDescriptors.ForPermutation(template),
            Algorithms = AlgorithmDescriptors.For<PermutationChromosome>(),
        };
    }

    /// <summary>
    /// Builds a symmetric Euclidean distance matrix from <paramref name="size"/> points drawn
    /// deterministically from <paramref name="seed"/>.
    /// </summary>
    private static double[,] CreateDistanceMatrix(int size, int seed)
    {
        Algorithms.Random.SeededRandom random = new(seed);
        double[] xs = new double[size];
        double[] ys = new double[size];

        for (int i = 0; i < size; i++)
        {
            xs[i] = random.NextDouble();
            ys[i] = random.NextDouble();
        }

        double[,] distances = new double[size, size];
        for (int i = 0; i < size; i++)
        {
            for (int j = i + 1; j < size; j++)
            {
                double dx = xs[i] - xs[j];
                double dy = ys[i] - ys[j];
                double distance = Math.Sqrt((dx * dx) + (dy * dy));
                distances[i, j] = distance;
                distances[j, i] = distance;
            }
        }

        return distances;
    }
}
