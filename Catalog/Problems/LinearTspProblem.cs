using Algorithms.Chromosomes;
using Algorithms.LossFunctions;
using Catalog.Descriptors;
using Core.Contracts;
using Parameters;

namespace Catalog.Problems;

/// <summary>
/// Linear travelling salesman: minimize the closed-tour length over cities spaced one unit apart on a line.
/// </summary>
/// <remarks>
/// City <c>i</c> sits at integer coordinate <c>i</c>, so the distance between cities <c>i</c> and
/// <c>j</c> is exactly <c>|i - j|</c>. The tour is still evaluated as a closed tour, therefore the
/// optimal loss for <c>size</c> cities is <c>2 * (size - 1)</c>: walk from one end to the other and
/// back again.
/// </remarks>
public sealed class LinearTspProblem : ProblemEntry<PermutationChromosome>
{
    private static readonly IReadOnlyList<ParameterSpec> Specs =
    [
        new("size", "Number of cities", ParameterKind.Int, "10", Min: 2, Max: 2000, Step: 1),
    ];

    /// <inheritdoc />
    public override string Key => "problem.lineartsp";

    /// <inheritdoc />
    public override string DisplayName => "Linear TSP";

    /// <inheritdoc />
    public override IReadOnlyList<ParameterSpec> Parameters => Specs;

    /// <inheritdoc />
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

    /// <summary>
    /// Builds a square matrix holding the ruler distances <c>|i - j|</c> between the cities.
    /// </summary>
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
