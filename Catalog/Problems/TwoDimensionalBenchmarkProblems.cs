using Algorithms.Chromosomes;
using Algorithms.LossFunctions;
using Core.Contracts;

namespace Catalog.Problems;

/// <summary>Booth: minimize a two-gene quadratic with its optimum at (1, 3).</summary>
public sealed class BoothProblem : FloatBenchmarkProblem
{
    /// <summary>Creates the Booth problem with the default gene bounds and size.</summary>
    public BoothProblem()
        : base("-10", "10", defaultSize: 2, minSize: 2)
    {
    }

    /// <inheritdoc />
    public override string Key => "problem.booth";

    /// <inheritdoc />
    public override string DisplayName => "Booth";

    /// <inheritdoc />
    protected override ILossFunction<FloatChromosome> CreateLoss() => new BoothLossFunction<FloatChromosome>();
}

/// <summary>Beale: minimize a two-gene polynomial with a narrow basin at (3, 0.5).</summary>
public sealed class BealeProblem : FloatBenchmarkProblem
{
    /// <summary>Creates the Beale problem with the default gene bounds and size.</summary>
    public BealeProblem()
        : base("-4.5", "4.5", defaultSize: 2, minSize: 2)
    {
    }

    /// <inheritdoc />
    public override string Key => "problem.beale";

    /// <inheritdoc />
    public override string DisplayName => "Beale";

    /// <inheritdoc />
    protected override ILossFunction<FloatChromosome> CreateLoss() => new BealeLossFunction<FloatChromosome>();
}

/// <summary>Himmelblau: minimize a two-gene function with four equally good global minima.</summary>
public sealed class HimmelblauProblem : FloatBenchmarkProblem
{
    /// <summary>Creates the Himmelblau problem with the default gene bounds and size.</summary>
    public HimmelblauProblem()
        : base("-5", "5", defaultSize: 2, minSize: 2)
    {
    }

    /// <inheritdoc />
    public override string Key => "problem.himmelblau";

    /// <inheritdoc />
    public override string DisplayName => "Himmelblau";

    /// <inheritdoc />
    protected override ILossFunction<FloatChromosome> CreateLoss() => new HimmelblauLossFunction<FloatChromosome>();
}

/// <summary>Easom: minimize a flat two-gene function with a single sharp minimum at (pi, pi).</summary>
public sealed class EasomProblem : FloatBenchmarkProblem
{
    /// <summary>Creates the Easom problem with the default gene bounds and size.</summary>
    public EasomProblem()
        : base("-100", "100", defaultSize: 2, minSize: 2)
    {
    }

    /// <inheritdoc />
    public override string Key => "problem.easom";

    /// <inheritdoc />
    public override string DisplayName => "Easom";

    /// <inheritdoc />
    protected override ILossFunction<FloatChromosome> CreateLoss() => new EasomLossFunction<FloatChromosome>();
}

/// <summary>Bukin: minimize a two-gene function with a narrow ridge along the first gene.</summary>
/// <remarks>
/// The classic Bukin N.6 domain is the box <c>x0 in [-15, -5]</c>, <c>x1 in [-3, 3]</c> around the
/// optimum <c>(-10, 1)</c>. A <see cref="FloatChromosome"/> shares one bound pair across all genes,
/// so the default range is widened to <c>[-15, 5]</c>: it still contains the optimum and the whole
/// valid <c>x0</c> range, while keeping <c>x1</c> free to reach its optimum.
/// </remarks>
public sealed class BukinProblem : FloatBenchmarkProblem
{
    /// <summary>Creates the Bukin problem with the default gene bounds and size.</summary>
    public BukinProblem()
        : base("-15", "5", defaultSize: 2, minSize: 2)
    {
    }

    /// <inheritdoc />
    public override string Key => "problem.bukin";

    /// <inheritdoc />
    public override string DisplayName => "Bukin";

    /// <inheritdoc />
    protected override ILossFunction<FloatChromosome> CreateLoss() => new BukinLossFunction<FloatChromosome>();
}

/// <summary>Matyas: minimize a smooth convex two-gene quadratic with its optimum at the origin.</summary>
public sealed class MatyasProblem : FloatBenchmarkProblem
{
    /// <summary>Creates the Matyas problem with the default gene bounds and size.</summary>
    public MatyasProblem()
        : base("-10", "10", defaultSize: 2, minSize: 2)
    {
    }

    /// <inheritdoc />
    public override string Key => "problem.matyas";

    /// <inheritdoc />
    public override string DisplayName => "Matyas";

    /// <inheritdoc />
    protected override ILossFunction<FloatChromosome> CreateLoss() => new MatyasLossFunction<FloatChromosome>();
}
