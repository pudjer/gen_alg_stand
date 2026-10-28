using Algorithms.Chromosomes;
using Algorithms.LossFunctions;
using Catalog.Descriptors;
using Core.Contracts;
using Parameters;

namespace Catalog.Problems;

/// <summary>Inversion count: sort a permutation by minimizing its Kendall-tau distance to the identity.</summary>
public sealed class InversionCountProblem : ProblemEntry<PermutationChromosome>
{
    private static readonly IReadOnlyList<ParameterSpec> Specs =
    [
        new("size", "Permutation length", ParameterKind.Int, "12", Min: 1, Max: 500, Step: 1),
    ];

    /// <inheritdoc />
    public override string Key => "problem.inversions";

    /// <inheritdoc />
    public override string DisplayName => "Inversion count";

    /// <inheritdoc />
    public override IReadOnlyList<ParameterSpec> Parameters => Specs;

    /// <inheritdoc />
    protected override ProblemOperators<PermutationChromosome> BuildOperators(ParameterSet problemParameters)
    {
        PermutationChromosome template = PermutationChromosome.Identity(problemParameters.GetInt("size"));
        ILossFunction<PermutationChromosome> loss = new InversionCountLossFunction<PermutationChromosome>();

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
}
