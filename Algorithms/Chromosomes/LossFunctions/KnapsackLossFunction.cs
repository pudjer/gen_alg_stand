using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.LossFunctions;


public sealed class KnapsackLossFunction<TChromosome> : ILossFunction<TChromosome>
    where TChromosome : IBinaryChromosome<TChromosome>
{
    private readonly int[] _weights;
    private readonly int[] _values;
    private readonly int _capacity;

    public KnapsackLossFunction(int itemCount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(itemCount, 1, nameof(itemCount));

        _weights = new int[itemCount];
        _values = new int[itemCount];

        for (int i = 0; i < itemCount; i++)
        {
            _weights[i] = ((i * 7) % 10) + 1;
            _values[i] = ((i * 13) % 20) + 1;
        }

        _capacity = _weights.Sum() / 2;
    }

    public KnapsackLossFunction(IReadOnlyList<int> weights, IReadOnlyList<int> values, int capacity)
    {
        ArgumentNullException.ThrowIfNull(weights);
        ArgumentNullException.ThrowIfNull(values);

        if (weights.Count == 0 || weights.Count != values.Count)
        {
            throw new ArgumentException("Weights and values must be non-empty and have the same length.");
        }

        ArgumentOutOfRangeException.ThrowIfNegative(capacity, nameof(capacity));

        _weights = [.. weights];
        _values = [.. values];
        _capacity = capacity;
    }

    public double Evaluate(TChromosome chromosome)
    {
        ArgumentNullException.ThrowIfNull(chromosome);

        if (chromosome.Length != _weights.Length)
        {
            throw new ArgumentException(
                $"Chromosome length {chromosome.Length} does not match item count {_weights.Length}.",
                nameof(chromosome));
        }

        int weight = 0;
        int value = 0;

        for (int i = 0; i < chromosome.Length; i++)
        {
            if (!chromosome[i])
            {
                continue;
            }

            weight += _weights[i];
            value += _values[i];
        }

        int overflow = Math.Max(0, weight - _capacity);
        return -value + (overflow * 1000d);
    }
}