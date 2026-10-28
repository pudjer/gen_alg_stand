using Algorithms.Orchestration;
using Core.Contracts;
using Implementations.Common;
using Parameters;
using Setup;

namespace Implementations.Modules;

internal static class AlgorithmParameters
{
    internal static readonly Parameter<int> Generations = Params.Int("generations", 0);

    internal static readonly Parameter<int> EliteCount = Params.Int("eliteCount", 0);
}

public sealed class GenerationalAlgorithmFactory<TChromosome>
    : IModuleFactory<IAlgorithm<TChromosome>>
    where TChromosome : IChromosome<TChromosome>
{
    public string Name => "Generational";

    public IReadOnlyList<IParameter> Parameters { get; } = [];

    public IAlgorithm<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);

        return new GenerationalAlgorithm<TChromosome>(
            new GenerationalOptions(
                MaxGenerations: values.Read(AlgorithmParameters.Generations, 1000),
                EliteCount: values.Read(AlgorithmParameters.EliteCount, 1),
                MaintainPopulationSize: true));
    }
}

public sealed class IslandModelAlgorithmFactory<TChromosome>
    : IModuleFactory<IAlgorithm<TChromosome>>
    where TChromosome : IChromosome<TChromosome>
{
    private static readonly Parameter<int> IslandCount = Params.Int("islandCount", 1);
    private static readonly Parameter<int> MigrationInterval = Params.Int("migrationInterval", 0);
    private static readonly Parameter<int> MigrantsPerIsland = Params.Int("migrantsPerIsland", 0);

    public string Name => "Island model";

    public IReadOnlyList<IParameter> Parameters { get; } =
        [IslandCount, MigrationInterval, MigrantsPerIsland];

    public IAlgorithm<TChromosome> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);

        return new IslandModelAlgorithm<TChromosome>(
            new IslandOptions(
                IslandCount: values.Read(IslandCount, 4),
                MaxGenerations: values.Read(AlgorithmParameters.Generations, 1000),
                MigrationInterval: values.Read(MigrationInterval, 10),
                MigrantsPerIsland: values.Read(MigrantsPerIsland, 1),
                EliteCount: values.Read(AlgorithmParameters.EliteCount, 1),
                MaintainPopulationSize: true,
                MaxDegreeOfParallelism: -1));
    }
}