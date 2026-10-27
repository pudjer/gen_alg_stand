using System.Numerics;

namespace Algorithms.Chromosomes;

/// <summary>A genotype whose genes are numbers constrained to [MinValue, MaxValue].</summary>
/// <typeparam name="TSelf">The concrete chromosome type.</typeparam>
/// <typeparam name="TNumber">The numeric gene type.</typeparam>
public interface INumericChromosome<TSelf, TNumber> : IGenotypeChromosome<TSelf, TNumber>
    where TSelf : INumericChromosome<TSelf, TNumber>
    where TNumber : System.Numerics.INumber<TNumber>
{
    /// <summary>Inclusive lower bound for gene values.</summary>
    TNumber MinValue { get; }

    /// <summary>Inclusive upper bound for gene values.</summary>
    TNumber MaxValue { get; }
}

/// <summary>Numeric chromosome whose genes are 32-bit integers.</summary>
/// <typeparam name="TSelf">The concrete chromosome type.</typeparam>
public interface IIntChromosome<TSelf> : INumericChromosome<TSelf, int>
    where TSelf : IIntChromosome<TSelf>;

/// <summary>Numeric chromosome whose genes are double-precision floats.</summary>
/// <typeparam name="TSelf">The concrete chromosome type.</typeparam>
public interface IFloatChromosome<TSelf> : INumericChromosome<TSelf, double>
    where TSelf : IFloatChromosome<TSelf>;
