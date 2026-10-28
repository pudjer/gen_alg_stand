using Algorithms.Chromosomes;
using Algorithms.LossFunctions;
using Catalog.Descriptors;
using Core.Contracts;
using Parameters;

namespace Catalog.Problems;

public sealed class LongestRunProblem : ProblemEntry<BinaryChromosome>
{
    private static readonly IReadOnlyList<ParameterSpec> Specs =
    [
        new("size", "Chromosome length", ParameterKind.Int, "30", Min: 1, Max: 10000, Step: 1),
    ];

    public override string Key => "problem.longestrun";

    public override string DisplayName => "Longest run";

    public override IReadOnlyList<ParameterSpec> Parameters => Specs;

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
