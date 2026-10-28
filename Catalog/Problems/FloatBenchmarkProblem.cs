using System.Globalization;
using Algorithms.Chromosomes;
using Algorithms.LossFunctions;
using Catalog.Descriptors;
using Core.Contracts;
using Parameters;

namespace Catalog.Problems;

public abstract class FloatBenchmarkProblem : ProblemEntry<FloatChromosome>
{
    private readonly IReadOnlyList<ParameterSpec> _specs;

    protected FloatBenchmarkProblem(string defaultMin, string defaultMax, int defaultSize, int minSize)
    {
        _specs =
        [
            new("size", "Number of genes", ParameterKind.Int, defaultSize.ToString(CultureInfo.InvariantCulture), Min: minSize, Max: 1000, Step: 1),
            new("minValue", "Gene lower bound", ParameterKind.Double, defaultMin, Min: -1000, Max: 1000, Step: 0.1),
            new("maxValue", "Gene upper bound", ParameterKind.Double, defaultMax, Min: -1000, Max: 1000, Step: 0.1),
        ];
    }

    public sealed override IReadOnlyList<ParameterSpec> Parameters => _specs;

    protected abstract ILossFunction<FloatChromosome> CreateLoss();

    protected sealed override ProblemOperators<FloatChromosome> BuildOperators(ParameterSet problemParameters)
    {
        FloatChromosome template = ProblemTemplates.FloatTemplate(problemParameters);
        ILossFunction<FloatChromosome> loss = CreateLoss();

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
