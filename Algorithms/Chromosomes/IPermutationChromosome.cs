using Core.Contracts;

namespace Algorithms.Chromosomes;

public interface IOrderedChromosome<TSelf> : IGenotypeChromosome<TSelf, int>
    where TSelf : IOrderedChromosome<TSelf>;

public interface IPermutationChromosome<TSelf> : IOrderedChromosome<TSelf>
    where TSelf : IPermutationChromosome<TSelf>;
