using Core.Contracts;

namespace Algorithms.Chromosomes;

/// <summary>Genotype whose integer genes are distinct; order carries meaning (scheduling, routing).</summary>
public interface IOrderedChromosome<TSelf> : IGenotypeChromosome<TSelf, int>
    where TSelf : IOrderedChromosome<TSelf>;

/// <summary>Ordered chromosome whose genes are a permutation of 0..Length-1 (TSP, N-Queens).</summary>
public interface IPermutationChromosome<TSelf> : IOrderedChromosome<TSelf>
    where TSelf : IPermutationChromosome<TSelf>;
