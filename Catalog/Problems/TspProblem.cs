using Algorithms.Chromosomes;
using Algorithms.LossFunctions;
using Catalog.Descriptors;
using Core.Contracts;
using Parameters;

namespace Catalog.Problems;

public sealed class TspProblem : ProblemEntry<PermutationChromosome>
{
    private static readonly IReadOnlyList<ParameterSpec> Specs =
    [
        new("size", "Number of cities", ParameterKind.Int, "10", Min: 2, Max: 500, Step: 1),
        new("matrixSeed", "Distance matrix seed", ParameterKind.Int, "7", Min: int.MinValue, Max: int.MaxValue, Step: 1),
    ];

    public override string Key => "problem.tsp";

    public override string DisplayName => "Travelling salesman";

    public override IReadOnlyList<ParameterSpec> Parameters => Specs;

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
