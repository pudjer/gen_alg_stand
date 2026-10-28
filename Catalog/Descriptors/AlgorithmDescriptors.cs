using Algorithms.Orchestration;
using Algorithms.Random;
using Core.Contracts;
using Parameters;

namespace Catalog.Descriptors;

public static class AlgorithmDescriptors
{
    public static IReadOnlyList<OperatorDescriptor> For<TSelf>(string keyPrefix = "algorithm")
        where TSelf : IChromosome<TSelf>
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(keyPrefix);

        return
        [
            new($"{keyPrefix}.generational", "Generational algorithm", OperatorDescriptor.Specs(
                    new ParameterSpec("maxGenerations", "Maximum generations", ParameterKind.Int, "500", Min: 0, Max: 100000, Step: 10),
                    new ParameterSpec("eliteCount", "Elite count", ParameterKind.Int, "1", Min: 0, Max: 1000, Step: 1),
                    new ParameterSpec("maintainPopulationSize", "Maintain population size", ParameterKind.Bool, "true")),
                p =>
                {
                    GenerationalOptions options = new(
                        p.GetInt("maxGenerations"),
                        p.GetInt("eliteCount"),
                        p.GetBool("maintainPopulationSize"));

                    return new AlgorithmFactory(seed => new GenerationalAlgorithm<TSelf>(new SeededRandom(seed), options));
                }),
            new($"{keyPrefix}.island", "Island model algorithm", OperatorDescriptor.Specs(
                    new ParameterSpec("islandCount", "Island count", ParameterKind.Int, "4", Min: 1, Max: 64, Step: 1),
                    new ParameterSpec("maxGenerations", "Maximum generations", ParameterKind.Int, "250", Min: 0, Max: 100000, Step: 10),
                    new ParameterSpec("migrationInterval", "Migration interval", ParameterKind.Int, "10", Min: 0, Max: 10000, Step: 1),
                    new ParameterSpec("migrantsPerIsland", "Migrants per island", ParameterKind.Int, "1", Min: 0, Max: 1000, Step: 1),
                    new ParameterSpec("eliteCount", "Elite count", ParameterKind.Int, "1", Min: 0, Max: 1000, Step: 1),
                    new ParameterSpec("maintainPopulationSize", "Maintain population size", ParameterKind.Bool, "true"),
                    new ParameterSpec(
                        "maxDegreeOfParallelism",
                        "Maximum parallelism",
                        ParameterKind.Int,
                        "1",
                        Description: "Upper bound on concurrently evolving islands; -1 means no limit.",
                        Min: -1,
                        Max: 64,
                        Step: 1)),
                p =>
                {
                    IslandOptions options = new(
                        p.GetInt("islandCount"),
                        p.GetInt("maxGenerations"),
                        p.GetInt("migrationInterval"),
                        p.GetInt("migrantsPerIsland"),
                        p.GetInt("eliteCount"),
                        p.GetBool("maintainPopulationSize"),
                        RandomFactory: null,
                        MaxDegreeOfParallelism: p.GetInt("maxDegreeOfParallelism"));

                    return new AlgorithmFactory(seed => new IslandModelAlgorithm<TSelf>(new SeededRandom(seed), options));
                }),
        ];
    }
}