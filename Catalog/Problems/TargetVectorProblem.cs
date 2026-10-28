using Algorithms.Chromosomes;
using Algorithms.LossFunctions;
using Catalog.Descriptors;
using Core.Contracts;
using Parameters;

namespace Catalog.Problems;

/// <summary>
/// Target vector: minimize the mean squared error to a fixed target vector.
/// </summary>
/// <remarks>
/// The target vector is generated deterministically from <c>targetSeed</c> by drawing each component
/// uniformly from the problem's gene range, so the same seed always yields the same target.
/// </remarks>
public sealed class TargetVectorProblem : ProblemEntry<FloatChromosome>
{
    private static readonly IReadOnlyList<ParameterSpec> Specs =
    [
        new("size", "Number of genes", ParameterKind.Int, "10", Min: 1, Max: 1000, Step: 1),
        new("minValue", "Gene lower bound", ParameterKind.Double, "-5", Min: -1000, Max: 1000, Step: 0.1),
        new("maxValue", "Gene upper bound", ParameterKind.Double, "5", Min: -1000, Max: 1000, Step: 0.1),
        new("targetSeed", "Target vector seed", ParameterKind.Int, "13", Min: int.MinValue, Max: int.MaxValue, Step: 1),
    ];

    /// <inheritdoc />
    public override string Key => "problem.targetvector";

    /// <inheritdoc />
    public override string DisplayName => "Target vector";

    /// <inheritdoc />
    public override IReadOnlyList<ParameterSpec> Parameters => Specs;

    /// <inheritdoc />
    protected override ProblemOperators<FloatChromosome> BuildOperators(ParameterSet problemParameters)
    {
        FloatChromosome template = ProblemTemplates.FloatTemplate(problemParameters);
        double min = problemParameters.GetDouble("minValue");
        double max = problemParameters.GetDouble("maxValue");
        int seed = problemParameters.GetInt("targetSeed");

        ILossFunction<FloatChromosome> loss = new TargetVectorLossFunction<FloatChromosome>(
            CreateTarget(template.Length, min, max, seed));

        return new ProblemOperators<FloatChromosome>
        {
            LossFunction = loss,
            Crossovers = [.. CrossoverDescriptors.For<FloatChromosome, double>(), .. CrossoverDescriptors.ForFloat<FloatChromosome>()],
            Mutations = MutationDescriptors.ForFloat<FloatChromosome>(),
            Selections = SelectionDescriptors.For(loss),
            InitialPopulations = InitialPopulationDescriptors.ForFloat(template),
            Algorithms = AlgorithmDescriptors.For<FloatChromosome>(),
        };
    }

    private static double[] CreateTarget(int size, double min, double max, int seed)
    {
        Algorithms.Random.SeededRandom random = new(seed);
        double[] target = new double[size];

        for (int i = 0; i < size; i++)
        {
            target[i] = min + (random.NextDouble() * (max - min));
        }

        return target;
    }
}
