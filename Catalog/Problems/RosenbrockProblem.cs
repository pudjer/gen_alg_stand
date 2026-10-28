using Algorithms.Chromosomes;
using Algorithms.LossFunctions;
using Catalog.Descriptors;
using Core.Contracts;
using Parameters;

namespace Catalog.Problems;

/// <summary>Rosenbrock: minimize the banana function over consecutive float gene pairs.</summary>
public sealed class RosenbrockProblem : ProblemEntry<FloatChromosome>
{
    private static readonly IReadOnlyList<ParameterSpec> Specs =
    [
        new("size", "Number of genes", ParameterKind.Int, "2", Min: 2, Max: 1000, Step: 1),
        new("minValue", "Gene lower bound", ParameterKind.Double, "-2", Min: -1000, Max: 1000, Step: 0.1),
        new("maxValue", "Gene upper bound", ParameterKind.Double, "2", Min: -1000, Max: 1000, Step: 0.1),
    ];

    /// <inheritdoc />
    public override string Key => "problem.rosenbrock";

    /// <inheritdoc />
    public override string DisplayName => "Rosenbrock";

    /// <inheritdoc />
    public override IReadOnlyList<ParameterSpec> Parameters => Specs;

    /// <inheritdoc />
    protected override ProblemOperators<FloatChromosome> BuildOperators(ParameterSet problemParameters)
    {
        FloatChromosome template = ProblemTemplates.FloatTemplate(problemParameters);
        ILossFunction<FloatChromosome> loss = new RosenbrockLossFunction<FloatChromosome>();

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
}
