using Algorithms.Chromosomes;
using Algorithms.LossFunctions;
using Catalog.Descriptors;
using Core.Contracts;
using Parameters;

namespace Catalog.Problems;

/// <summary>Longest run: maximize the longest consecutive run of true genes in a binary chromosome.</summary>
public sealed class LongestRunProblem : ProblemEntry<BinaryChromosome>
{
    private static readonly IReadOnlyList<ParameterSpec> Specs =
    [
        new("size", "Chromosome length", ParameterKind.Int, "30", Min: 1, Max: 10000, Step: 1),
    ];

    /// <inheritdoc />
    public override string Key => "problem.longestrun";

    /// <inheritdoc />
    public override string DisplayName => "Longest run";

    /// <inheritdoc />
    public override IReadOnlyList<ParameterSpec> Parameters => Specs;

    /// <inheritdoc />
    protected override ProblemOperators<BinaryChromosome> BuildOperators(ParameterSet problemParameters)
    {
        int size = problemParameters.GetInt("size");
        BinaryChromosome template = new(new bool[size]);
        ILossFunction<BinaryChromosome> loss = new LongestRunLossFunction<BinaryChromosome>();

        return new ProblemOperators<BinaryChromosome>
        {
            LossFunction = loss,
            Crossovers = CrossoverDescriptors.For<BinaryChromosome, bool>(),
            Mutations = MutationDescriptors.ForBinary<BinaryChromosome>(),
            Selections = SelectionDescriptors.For(loss),
            InitialPopulations = InitialPopulationDescriptors.ForBinary(template),
            Algorithms = AlgorithmDescriptors.For<BinaryChromosome>(),
        };
    }
}
