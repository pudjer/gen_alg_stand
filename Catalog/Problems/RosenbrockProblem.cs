using Algorithms.Chromosomes;
using Algorithms.LossFunctions;
using Catalog.Descriptors;
using Core.Contracts;
using Parameters;

namespace Catalog.Problems;

public sealed class RosenbrockProblem : ProblemEntry<FloatChromosome>
{
    private static readonly IReadOnlyList<ParameterSpec> Specs =
    [
        new("size", "Number of genes", ParameterKind.Int, "2", Min: 2, Max: 1000, Step: 1),
        new("minValue", "Gene lower bound", ParameterKind.Double, "-2", Min: -1000, Max: 1000, Step: 0.1),
        new("maxValue", "Gene upper bound", ParameterKind.Double, "2", Min: -1000, Max: 1000, Step: 0.1),
    ];

    public override string Key => "problem.rosenbrock";

    public override string DisplayName => "Rosenbrock";

    public override IReadOnlyList<ParameterSpec> Parameters => Specs;

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
