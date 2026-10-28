using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.LossFunctions;

public sealed class BoothLossFunction<TChromosome> : ILossFunction<TChromosome>
    where TChromosome : IFloatChromosome<TChromosome>
{
    public BoothLossFunction()
    {
    }

    public double Evaluate(TChromosome chromosome)
    {
        ArgumentNullException.ThrowIfNull(chromosome);

        if (chromosome.Length < 2)
        {
            return 0d;
        }

        double x0 = chromosome[0];
        double x1 = chromosome[1];
        double firstTerm = x0 + (2d * x1) - 7d;
        double secondTerm = (2d * x0) + x1 - 5d;

        return (firstTerm * firstTerm) + (secondTerm * secondTerm);
    }
}

public sealed class BealeLossFunction<TChromosome> : ILossFunction<TChromosome>
    where TChromosome : IFloatChromosome<TChromosome>
{
    public BealeLossFunction()
    {
    }

    public double Evaluate(TChromosome chromosome)
    {
        ArgumentNullException.ThrowIfNull(chromosome);

        if (chromosome.Length < 2)
        {
            return 0d;
        }

        double x0 = chromosome[0];
        double x1 = chromosome[1];
        double firstTerm = 1.5d - x0 + (x0 * x1);
        double secondTerm = 2.25d - x0 + (x0 * x1 * x1);
        double thirdTerm = 2.625d - x0 + (x0 * x1 * x1 * x1);

        return (firstTerm * firstTerm) + (secondTerm * secondTerm) + (thirdTerm * thirdTerm);
    }
}

public sealed class HimmelblauLossFunction<TChromosome> : ILossFunction<TChromosome>
    where TChromosome : IFloatChromosome<TChromosome>
{
    public HimmelblauLossFunction()
    {
    }

    public double Evaluate(TChromosome chromosome)
    {
        ArgumentNullException.ThrowIfNull(chromosome);

        if (chromosome.Length < 2)
        {
            return 0d;
        }

        double x0 = chromosome[0];
        double x1 = chromosome[1];
        double firstTerm = (x0 * x0) + x1 - 11d;
        double secondTerm = x0 + (x1 * x1) - 7d;

        return (firstTerm * firstTerm) + (secondTerm * secondTerm);
    }
}

public sealed class EasomLossFunction<TChromosome> : ILossFunction<TChromosome>
    where TChromosome : IFloatChromosome<TChromosome>
{
    public EasomLossFunction()
    {
    }

    public double Evaluate(TChromosome chromosome)
    {
        ArgumentNullException.ThrowIfNull(chromosome);

        if (chromosome.Length < 2)
        {
            return 0d;
        }

        double x0 = chromosome[0];
        double x1 = chromosome[1];
        double offset0 = x0 - Math.PI;
        double offset1 = x1 - Math.PI;
        double exponent = -((offset0 * offset0) + (offset1 * offset1));

        return -Math.Cos(x0) * Math.Cos(x1) * Math.Exp(exponent);
    }
}

public sealed class BukinLossFunction<TChromosome> : ILossFunction<TChromosome>
    where TChromosome : IFloatChromosome<TChromosome>
{
    public BukinLossFunction()
    {
    }

    public double Evaluate(TChromosome chromosome)
    {
        ArgumentNullException.ThrowIfNull(chromosome);

        if (chromosome.Length < 2)
        {
            return 0d;
        }

        double x0 = chromosome[0];
        double x1 = chromosome[1];
        double firstTerm = 100d * Math.Sqrt(Math.Abs(x1 - (0.01d * x0 * x0)));
        double secondTerm = 0.01d * Math.Abs(x0 + 10d);

        return firstTerm + secondTerm;
    }
}

public sealed class MatyasLossFunction<TChromosome> : ILossFunction<TChromosome>
    where TChromosome : IFloatChromosome<TChromosome>
{
    public MatyasLossFunction()
    {
    }

    public double Evaluate(TChromosome chromosome)
    {
        ArgumentNullException.ThrowIfNull(chromosome);

        if (chromosome.Length < 2)
        {
            return 0d;
        }

        double x0 = chromosome[0];
        double x1 = chromosome[1];

        return (0.26d * ((x0 * x0) + (x1 * x1))) - (0.48d * x0 * x1);
    }
}
