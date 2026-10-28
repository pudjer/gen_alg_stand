using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.LossFunctions;

public sealed class AckleyLossFunction<TChromosome> : ILossFunction<TChromosome>
    where TChromosome : IFloatChromosome<TChromosome>
{
    public AckleyLossFunction()
    {
    }

    public double Evaluate(TChromosome chromosome)
    {
        ArgumentNullException.ThrowIfNull(chromosome);

        int n = chromosome.Length;
        if (n == 0)
        {
            return 0d;
        }

        double sumSquares = 0d;
        double sumCosines = 0d;

        for (int i = 0; i < n; i++)
        {
            double value = chromosome[i];
            sumSquares += value * value;
            sumCosines += Math.Cos(2d * Math.PI * value);
        }

        double firstTerm = -20d * Math.Exp(-0.2d * Math.Sqrt(sumSquares / n));
        double secondTerm = -Math.Exp(sumCosines / n);

        return firstTerm + secondTerm + 20d + Math.E;
    }
}

public sealed class GriewankLossFunction<TChromosome> : ILossFunction<TChromosome>
    where TChromosome : IFloatChromosome<TChromosome>
{
    public GriewankLossFunction()
    {
    }

    public double Evaluate(TChromosome chromosome)
    {
        ArgumentNullException.ThrowIfNull(chromosome);

        int n = chromosome.Length;
        if (n == 0)
        {
            return 0d;
        }

        double sumSquares = 0d;
        double productCosines = 1d;

        for (int i = 0; i < n; i++)
        {
            double value = chromosome[i];
            sumSquares += (value * value) / 4000d;
            productCosines *= Math.Cos(value / Math.Sqrt(i + 1));
        }

        return 1d + sumSquares - productCosines;
    }
}

public sealed class SchwefelLossFunction<TChromosome> : ILossFunction<TChromosome>
    where TChromosome : IFloatChromosome<TChromosome>
{
    public SchwefelLossFunction()
    {
    }

    public double Evaluate(TChromosome chromosome)
    {
        ArgumentNullException.ThrowIfNull(chromosome);

        int n = chromosome.Length;
        if (n == 0)
        {
            return 0d;
        }

        double sum = 0d;

        for (int i = 0; i < n; i++)
        {
            double value = chromosome[i];
            sum += value * Math.Sin(Math.Sqrt(Math.Abs(value)));
        }

        return 418.9828872724338d * n - sum;
    }
}
