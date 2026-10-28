using Algorithms.Chromosomes;
using Algorithms.LossFunctions;
using Core.Contracts;

namespace Catalog.Problems;

public sealed class AckleyProblem : FloatBenchmarkProblem
{
    public AckleyProblem()
        : base("-5", "5", defaultSize: 10, minSize: 1)
    {
    }

    public override string Key => "problem.ackley";

    public override string DisplayName => "Ackley";

    protected override ILossFunction<FloatChromosome> CreateLoss() => new AckleyLossFunction<FloatChromosome>();
}

public sealed class GriewankProblem : FloatBenchmarkProblem
{
    public GriewankProblem()
        : base("-600", "600", defaultSize: 10, minSize: 1)
    {
    }

    public override string Key => "problem.griewank";

    public override string DisplayName => "Griewank";

    protected override ILossFunction<FloatChromosome> CreateLoss() => new GriewankLossFunction<FloatChromosome>();
}

public sealed class SchwefelProblem : FloatBenchmarkProblem
{
    public SchwefelProblem()
        : base("-500", "500", defaultSize: 10, minSize: 1)
    {
    }

    public override string Key => "problem.schwefel";

    public override string DisplayName => "Schwefel";

    protected override ILossFunction<FloatChromosome> CreateLoss() => new SchwefelLossFunction<FloatChromosome>();
}
