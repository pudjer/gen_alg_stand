using System.Numerics;

namespace Algorithms.Chromosomes;

public interface INumericChromosome<TSelf, TNumber> : IGenotypeChromosome<TSelf, TNumber>
    where TSelf : INumericChromosome<TSelf, TNumber>
    where TNumber : System.Numerics.INumber<TNumber>
{
    TNumber MinValue { get; }

    TNumber MaxValue { get; }
}

public interface IIntChromosome<TSelf> : INumericChromosome<TSelf, int>
    where TSelf : IIntChromosome<TSelf>;

public interface IFloatChromosome<TSelf> : INumericChromosome<TSelf, double>
    where TSelf : IFloatChromosome<TSelf>;
