using Algorithms.Chromosomes;
using Algorithms.LossFunctions;
using Core.Contracts;

namespace Catalog.Problems;

/// <summary>Ackley: minimize a multimodal function with a narrow global basin at the origin.</summary>
public sealed class AckleyProblem : FloatBenchmarkProblem
{
    /// <summary>Creates the Ackley problem with the default gene bounds and size.</summary>
    public AckleyProblem()
        : base("-5", "5", defaultSize: 10, minSize: 1)
    {
    }

    /// <inheritdoc />
    public override string Key => "problem.ackley";

    /// <inheritdoc />
    public override string DisplayName => "Ackley";

    /// <inheritdoc />
    protected override ILossFunction<FloatChromosome> CreateLoss() => new AckleyLossFunction<FloatChromosome>();
}

/// <summary>Griewank: minimize a product-cosine function riddled with local minima.</summary>
public sealed class GriewankProblem : FloatBenchmarkProblem
{
    /// <summary>Creates the Griewank problem with the default gene bounds and size.</summary>
    public GriewankProblem()
        : base("-600", "600", defaultSize: 10, minSize: 1)
    {
    }

    /// <inheritdoc />
    public override string Key => "problem.griewank";

    /// <inheritdoc />
    public override string DisplayName => "Griewank";

    /// <inheritdoc />
    protected override ILossFunction<FloatChromosome> CreateLoss() => new GriewankLossFunction<FloatChromosome>();
}

/// <summary>Schwefel: minimize a deceiving multimodal function with the optimum shifted from the origin.</summary>
public sealed class SchwefelProblem : FloatBenchmarkProblem
{
    /// <summary>Creates the Schwefel problem with the default gene bounds and size.</summary>
    public SchwefelProblem()
        : base("-500", "500", defaultSize: 10, minSize: 1)
    {
    }

    /// <inheritdoc />
    public override string Key => "problem.schwefel";

    /// <inheritdoc />
    public override string DisplayName => "Schwefel";

    /// <inheritdoc />
    protected override ILossFunction<FloatChromosome> CreateLoss() => new SchwefelLossFunction<FloatChromosome>();
}
