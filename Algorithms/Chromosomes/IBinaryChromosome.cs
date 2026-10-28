using Core.Contracts;

namespace Algorithms.Chromosomes;

public interface IBinaryChromosome<TSelf> : IGenotypeChromosome<TSelf, bool>
    where TSelf : IBinaryChromosome<TSelf>;
