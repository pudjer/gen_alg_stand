using Core.Contracts;

namespace Algorithms.Chromosomes;

/// <summary>Chromosome whose genes are bits.</summary>
public interface IBinaryChromosome<TSelf> : IGenotypeChromosome<TSelf, bool>
    where TSelf : IBinaryChromosome<TSelf>;
