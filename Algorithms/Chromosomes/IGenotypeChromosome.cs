using System.Collections.Generic;
using Core.Contracts;

namespace Algorithms.Chromosomes;

public interface IGenotypeChromosome<TSelf, TGene> : IChromosome<TSelf>
    where TSelf : IGenotypeChromosome<TSelf, TGene>
{
    int Length { get; }

    TGene this[int index] { get; }

    IReadOnlyList<TGene> Genes { get; }

    TSelf WithGenes(IReadOnlyList<TGene> genes);
}
