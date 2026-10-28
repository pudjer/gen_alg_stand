using Algorithms.Chromosomes;
using Algorithms.LossFunctions;
using Catalog.Descriptors;
using Core.Contracts;
using Parameters;

namespace Catalog.Problems;

/// <summary>Sum target: make the integer genes add up to a target total.</summary>
public sealed class SumTargetProblem : ProblemEntry<IntChromosome>
{
    private static readonly IReadOnlyList<ParameterSpec> Specs =
    [
        new("size", "Number of genes", ParameterKind.Int, "10", Min: 1, Max: 1000, Step: 1),
        new("minValue", "Gene lower bound", ParameterKind.Int, "0", Min: -1000000, Max: 1000000, Step: 1),
        new("maxValue", "Gene upper bound", ParameterKind.Int, "10", Min: -1000000, Max: 1000000, Step: 1),
        new("target", "Target sum", ParameterKind.Int, "50", Min: -100000000, Max: 100000000, Step: 1),
    ];

    /// <inheritdoc />
    public override string Key => "problem.sumtarget";

    /// <inheritdoc />
    public override string DisplayName => "Sum target";

    /// <inheritdoc />
    public override IReadOnlyList<ParameterSpec> Parameters => Specs;

    /// <inheritdoc />
    protected override ProblemOperators<IntChromosome> BuildOperators(ParameterSet problemParameters)
    {
        IntChromosome template = ProblemTemplates.IntTemplate(problemParameters);
        ILossFunction<IntChromosome> loss = new SumTargetLossFunction<IntChromosome>(problemParameters.GetInt("target"));

        return new ProblemOperators<IntChromosome>
        {
            LossFunction = loss,
            Crossovers = [.. CrossoverDescriptors.For<IntChromosome, int>(), .. CrossoverDescriptors.ForInt<IntChromosome>()],
            Mutations = MutationDescriptors.ForInt<IntChromosome>(),
            Selections = SelectionDescriptors.For(loss),
            InitialPopulations = InitialPopulationDescriptors.ForInt(template),
            Algorithms = AlgorithmDescriptors.For<IntChromosome>(),
        };
    }
}
