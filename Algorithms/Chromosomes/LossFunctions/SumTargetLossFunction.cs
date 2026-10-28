using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.LossFunctions;

public sealed class SumTargetLossFunction<TChromosome> : ILossFunction<TChromosome>
    where TChromosome : IIntChromosome<TChromosome>
{
    private readonly long _target;

    public SumTargetLossFunction(long target)
    {
        _target = target;
    }

    public double Evaluate(TChromosome chromosome)
    {
        ArgumentNullException.ThrowIfNull(chromosome);

        double sum = 0d;
        for (int i = 0; i < chromosome.Length; i++)
        {
            sum += chromosome[i];
        }

        return Math.Abs(sum - _target);
    }
}
