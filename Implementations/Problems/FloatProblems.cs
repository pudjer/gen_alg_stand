using Algorithms.Chromosomes;
using Core.Contracts;
using Implementations.Common;
using Implementations.Modules;
using Parameters;
using Setup;

namespace Implementations.Problems;

public abstract class FloatProblemFactoryBase : IProblemFactory<FloatChromosome>
{
    private static readonly Parameter<int> Length = Params.Int("length", 1);
    private static readonly Parameter<double> MinValue = Params.Double("minValue");
    private static readonly Parameter<double> MaxValue = Params.Double("maxValue");

    public IReadOnlyList<IParameter> Parameters { get; } = [Length, MinValue, MaxValue];

    protected virtual int DefaultLength => 10;

    protected virtual double DefaultMinValue => -5.12;

    protected virtual double DefaultMaxValue => 5.12;

    protected abstract IModuleFactory<ILossFunction<FloatChromosome>> CreateLoss(
        int length,
        double minValue,
        double maxValue);

    public IReadOnlyList<Variant<FloatChromosome>> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);

        int length = values.Read(Length, DefaultLength);
        double minValue = values.Read(MinValue, DefaultMinValue);
        double maxValue = values.Read(MaxValue, DefaultMaxValue);

        if (minValue > maxValue)
        {
            throw new ArgumentException("minValue must be less than or equal to maxValue.", nameof(values));
        }

        return new ProblemFactory<FloatChromosome>
        {
            LossFunctions = [CreateLoss(length, minValue, maxValue)],
            Selections = ModuleCatalog.Selections<FloatChromosome>(),
            Crossovers = ModuleCatalog.FloatCrossovers(),
            Mutations = ModuleCatalog.FloatMutations(),
            InitialPopulations = [new RandomFloatPopulationFactory(length, minValue, maxValue)],
        }.Create(values);
    }
}

public sealed class SphereProblemFactory : FloatProblemFactoryBase
{
    protected override IModuleFactory<ILossFunction<FloatChromosome>> CreateLoss(
        int length,
        double minValue,
        double maxValue) =>
        new SphereLossFactory<FloatChromosome>();
}

public sealed class RastriginProblemFactory : FloatProblemFactoryBase
{
    protected override IModuleFactory<ILossFunction<FloatChromosome>> CreateLoss(
        int length,
        double minValue,
        double maxValue) =>
        new RastriginLossFactory<FloatChromosome>();
}

public sealed class RosenbrockProblemFactory : FloatProblemFactoryBase
{
    protected override double DefaultMinValue => -2.048;

    protected override double DefaultMaxValue => 2.048;

    protected override IModuleFactory<ILossFunction<FloatChromosome>> CreateLoss(
        int length,
        double minValue,
        double maxValue) =>
        new RosenbrockLossFactory<FloatChromosome>();
}

public sealed class AckleyProblemFactory : FloatProblemFactoryBase
{
    protected override double DefaultMinValue => -32.768;

    protected override double DefaultMaxValue => 32.768;

    protected override IModuleFactory<ILossFunction<FloatChromosome>> CreateLoss(
        int length,
        double minValue,
        double maxValue) =>
        new AckleyLossFactory<FloatChromosome>();
}

public sealed class GriewankProblemFactory : FloatProblemFactoryBase
{
    protected override double DefaultMinValue => -600;

    protected override double DefaultMaxValue => 600;

    protected override IModuleFactory<ILossFunction<FloatChromosome>> CreateLoss(
        int length,
        double minValue,
        double maxValue) =>
        new GriewankLossFactory<FloatChromosome>();
}

public sealed class SchwefelProblemFactory : FloatProblemFactoryBase
{
    protected override double DefaultMinValue => -500;

    protected override double DefaultMaxValue => 500;

    protected override IModuleFactory<ILossFunction<FloatChromosome>> CreateLoss(
        int length,
        double minValue,
        double maxValue) =>
        new SchwefelLossFactory<FloatChromosome>();
}

public sealed class BoothProblemFactory : FloatProblemFactoryBase
{
    protected override int DefaultLength => 2;

    protected override double DefaultMinValue => -10;

    protected override double DefaultMaxValue => 10;

    protected override IModuleFactory<ILossFunction<FloatChromosome>> CreateLoss(
        int length,
        double minValue,
        double maxValue) =>
        new BoothLossFactory<FloatChromosome>();
}

public sealed class BealeProblemFactory : FloatProblemFactoryBase
{
    protected override int DefaultLength => 2;

    protected override double DefaultMinValue => -4.5;

    protected override double DefaultMaxValue => 4.5;

    protected override IModuleFactory<ILossFunction<FloatChromosome>> CreateLoss(
        int length,
        double minValue,
        double maxValue) =>
        new BealeLossFactory<FloatChromosome>();
}

public sealed class HimmelblauProblemFactory : FloatProblemFactoryBase
{
    protected override int DefaultLength => 2;

    protected override double DefaultMinValue => -5;

    protected override double DefaultMaxValue => 5;

    protected override IModuleFactory<ILossFunction<FloatChromosome>> CreateLoss(
        int length,
        double minValue,
        double maxValue) =>
        new HimmelblauLossFactory<FloatChromosome>();
}

public sealed class EasomProblemFactory : FloatProblemFactoryBase
{
    protected override int DefaultLength => 2;

    protected override double DefaultMinValue => -100;

    protected override double DefaultMaxValue => 100;

    protected override IModuleFactory<ILossFunction<FloatChromosome>> CreateLoss(
        int length,
        double minValue,
        double maxValue) =>
        new EasomLossFactory<FloatChromosome>();
}

public sealed class BukinProblemFactory : FloatProblemFactoryBase
{
    protected override int DefaultLength => 2;

    protected override double DefaultMinValue => -15;

    protected override double DefaultMaxValue => 15;

    protected override IModuleFactory<ILossFunction<FloatChromosome>> CreateLoss(
        int length,
        double minValue,
        double maxValue) =>
        new BukinLossFactory<FloatChromosome>();
}

public sealed class MatyasProblemFactory : FloatProblemFactoryBase
{
    protected override int DefaultLength => 2;

    protected override double DefaultMinValue => -10;

    protected override double DefaultMaxValue => 10;

    protected override IModuleFactory<ILossFunction<FloatChromosome>> CreateLoss(
        int length,
        double minValue,
        double maxValue) =>
        new MatyasLossFactory<FloatChromosome>();
}

public sealed class TargetVectorProblemFactory : FloatProblemFactoryBase
{
    protected override int DefaultLength => 2;

    protected override IModuleFactory<ILossFunction<FloatChromosome>> CreateLoss(
        int length,
        double minValue,
        double maxValue)
    {
        double targetValue = (minValue + maxValue) / 2;
        double[] target = Enumerable.Repeat(targetValue, length).ToArray();

        return new TargetVectorLossFactory<FloatChromosome>(target);
    }
}