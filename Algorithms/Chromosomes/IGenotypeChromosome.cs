using System.Collections.Generic;
using Core.Contracts;

namespace Algorithms.Chromosomes;

/// <summary>A chromosome represented as a fixed-length sequence of genes.</summary>
/// <typeparam name="TSelf">The concrete chromosome type.</typeparam>
/// <typeparam name="TGene">The gene type.</typeparam>
public interface IGenotypeChromosome<TSelf, TGene> : IChromosome<TSelf>
    where TSelf : IGenotypeChromosome<TSelf, TGene>
{
    /// <summary>Number of genes.</summary>
    int Length { get; }

    /// <summary>Gene at the given position.</summary>
    /// <param name="index">Zero-based gene position.</param>
    TGene this[int index] { get; }

    /// <summary>Read-only view over the genes; never mutated.</summary>
    IReadOnlyList<TGene> Genes { get; }

    /// <summary>Creates a chromosome with the same parameters and the supplied genes (copied).</summary>
    /// <param name="genes">The genes to copy into the new chromosome.</param>
    TSelf WithGenes(IReadOnlyList<TGene> genes);
}
