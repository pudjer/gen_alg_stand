using Algorithms.Chromosomes;
using Algorithms.LossFunctions;
using Core.Contracts;

namespace Catalog.Problems;

public sealed class BoothProblem : FloatBenchmarkProblem
{
    public BoothProblem()
        : base("-10", "10", defaultSize: 2, minSize: 2)
    {
    }

    public override string Key => "problem.booth";

    public override string DisplayName => "Booth";

    protected override ILossFunction<FloatChromosome> CreateLoss() => new BoothLossFunction<FloatChromosome>();
}

public sealed class BealeProblem : FloatBenchmarkProblem
{
    public BealeProblem()
        : base("-4.5", "4.5", defaultSize: 2, minSize: 2)
    {
    }

    public override string Key => "problem.beale";

    public override string DisplayName => "Beale";

    protected override ILossFunction<FloatChromosome> CreateLoss() => new BealeLossFunction<FloatChromosome>();
}

public sealed class HimmelblauProblem : FloatBenchmarkProblem
{
    public HimmelblauProblem()
        : base("-5", "5", defaultSize: 2, minSize: 2)
    {
    }

    public override string Key => "problem.himmelblau";

    public override string DisplayName => "Himmelblau";

    protected override ILossFunction<FloatChromosome> CreateLoss() => new HimmelblauLossFunction<FloatChromosome>();
}

public sealed class EasomProblem : FloatBenchmarkProblem
{
    public EasomProblem()
        : base("-100", "100", defaultSize: 2, minSize: 2)
    {
    }

    public override string Key => "problem.easom";

    public override string DisplayName => "Easom";

    protected override ILossFunction<FloatChromosome> CreateLoss() => new EasomLossFunction<FloatChromosome>();
}

public sealed class BukinProblem : FloatBenchmarkProblem
{
    public BukinProblem()
        : base("-15", "5", defaultSize: 2, minSize: 2)
    {
    }

    public override string Key => "problem.bukin";

    public override string DisplayName => "Bukin";

    protected override ILossFunction<FloatChromosome> CreateLoss() => new BukinLossFunction<FloatChromosome>();
}

public sealed class MatyasProblem : FloatBenchmarkProblem
{
    public MatyasProblem()
        : base("-10", "10", defaultSize: 2, minSize: 2)
    {
    }

    public override string Key => "problem.matyas";

    public override string DisplayName => "Matyas";

    protected override ILossFunction<FloatChromosome> CreateLoss() => new MatyasLossFunction<FloatChromosome>();
}
