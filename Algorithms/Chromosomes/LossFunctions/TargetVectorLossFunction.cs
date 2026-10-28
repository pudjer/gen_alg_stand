using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.LossFunctions;

public sealed class TargetVectorLossFunction<TChromosome> : ILossFunction<TChromosome>
    where TChromosome : IFloatChromosome<TChromosome>
{
    private readonly double[] _target;

    public TargetVectorLossFunction(IReadOnlyList<double> target)
    {
        ArgumentNullException.ThrowIfNull(target);

        _target = new double[target.Count];
        for (int i = 0; i < _target.Length; i++)
        {
            _target[i] = target[i];
        }
    }

    public double Evaluate(TChromosome chromosome)
    {
        ArgumentNullException.ThrowIfNull(chromosome);

        if (chromosome.Length != _target.Length)
        {
            throw new ArgumentException(
                $"Chromosome length {chromosome.Length} does not match target length {_target.Length}.",
                nameof(chromosome));
        }

        if (_target.Length == 0)
        {
            return 0d;
        }

        double sum = 0d;
        for (int i = 0; i < _target.Length; i++)
        {
            double difference = chromosome[i] - _target[i];
            sum += difference * difference;
        }

        return sum / _target.Length;
    }
}
