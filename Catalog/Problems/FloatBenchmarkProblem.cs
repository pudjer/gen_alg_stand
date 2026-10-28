using System.Globalization;
using Algorithms.Chromosomes;
using Algorithms.LossFunctions;
using Catalog.Descriptors;
using Core.Contracts;
using Parameters;

namespace Catalog.Problems;

/// <summary>Base class for float benchmark problems sharing the conventional size and gene-bound parameters.</summary>
public abstract class FloatBenchmarkProblem : ProblemEntry<FloatChromosome>
{
    private readonly IReadOnlyList<ParameterSpec> _specs;

    /// <summary>Initializes the benchmark with its default gene bounds and chromosome size.</summary>
    /// <param name="defaultMin">The default lower gene bound, as text.</param>
    /// <param name="defaultMax">The default upper gene bound, as text.</param>
    /// <param name="defaultSize">The default number of genes.</param>
    /// <param name="minSize">The smallest allowed number of genes.</param>
    protected FloatBenchmarkProblem(string defaultMin, string defaultMax, int defaultSize, int minSize)
    {
        _specs =
        [
            new("size", "Number of genes", ParameterKind.Int, defaultSize.ToString(CultureInfo.InvariantCulture), Min: minSize, Max: 1000, Step: 1),
            new("minValue", "Gene lower bound", ParameterKind.Double, defaultMin, Min: -1000, Max: 1000, Step: 0.1),
            new("maxValue", "Gene upper bound", ParameterKind.Double, defaultMax, Min: -1000, Max: 1000, Step: 0.1),
        ];
    }

    /// <inheritdoc />
    public sealed override IReadOnlyList<ParameterSpec> Parameters => _specs;

    /// <summary>Creates the loss function of the benchmark.</summary>
    /// <returns>The loss function evaluated over float chromosomes.</returns>
    protected abstract ILossFunction<FloatChromosome> CreateLoss();

    /// <inheritdoc />
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
