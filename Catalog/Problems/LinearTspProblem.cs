using Algorithms.Chromosomes;
using Algorithms.LossFunctions;
using Catalog.Descriptors;
using Core.Contracts;
using Parameters;

namespace Catalog.Problems;

public sealed class LinearTspProblem : ProblemEntry<PermutationChromosome>
{
    private static readonly IReadOnlyList<ParameterSpec> Specs =
    [
        new("size", "Number of cities", ParameterKind.Int, "10", Min: 2, Max: 2000, Step: 1),
    ];

    public override string Key => "problem.lineartsp";

    public override string DisplayName => "Linear TSP";

    public override IReadOnlyList<ParameterSpec> Parameters => Specs;

    protected override ProblemOperators<PermutationChromosome> BuildOperators(ParameterSet problemParameters)
    {
        int size = problemParameters.GetInt("size");

        PermutationChromosome template = PermutationChromosome.Identity(size);
        ILossFunction<PermutationChromosome> loss = new TspLossFunction<PermutationChromosome>(CreateRulerDistances(size));

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

    private static double[,] CreateRulerDistances(int size)
    {
        double[,] distances = new double[size, size];
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                distances[i, j] = Math.Abs(i - j);
            }
        }

        return distances;
    }
}
