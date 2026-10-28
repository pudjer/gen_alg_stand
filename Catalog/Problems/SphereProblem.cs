using Algorithms.Chromosomes;
using Algorithms.LossFunctions;
using Catalog.Descriptors;
using Core.Contracts;
using Parameters;

namespace Catalog.Problems;

/// <summary>Sphere: minimize the sum of squared genes of a float chromosome.</summary>
public sealed class SphereProblem : ProblemEntry<FloatChromosome>
{
    private static readonly IReadOnlyList<ParameterSpec> Specs =
    [
        new("size", "Number of genes", ParameterKind.Int, "10", Min: 1, Max: 1000, Step: 1),
        new("minValue", "Gene lower bound", ParameterKind.Double, "-5.12", Min: -1000, Max: 1000, Step: 0.1),
        new("maxValue", "Gene upper bound", ParameterKind.Double, "5.12", Min: -1000, Max: 1000, Step: 0.1),
    ];

    /// <inheritdoc />
    public override string Key => "problem.sphere";

    /// <inheritdoc />
    public override string DisplayName => "Sphere";

    /// <inheritdoc />
    public override IReadOnlyList<ParameterSpec> Parameters => Specs;

    /// <inheritdoc />
    protected override ProblemOperators<FloatChromosome> BuildOperators(ParameterSet problemParameters)
    {
        FloatChromosome template = ProblemTemplates.FloatTemplate(problemParameters);
        ILossFunction<FloatChromosome> loss = new SphereLossFunction<FloatChromosome>();

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
