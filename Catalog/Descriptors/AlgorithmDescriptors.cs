using Algorithms.Orchestration;
using Algorithms.Random;
using Core.Contracts;
using Parameters;

namespace Catalog.Descriptors;

/// <summary>Descriptors for the algorithms that drive a run.</summary>
/// <remarks>
/// Algorithm descriptors do not build the algorithm directly: the runner supplies the seed once it
/// knows the configuration, so each factory returns an <see cref="AlgorithmFactory"/>.
/// </remarks>
public static class AlgorithmDescriptors
{
    /// <summary>Gets the algorithms valid for <typeparamref name="TSelf"/>.</summary>
    /// <typeparam name="TSelf">Concrete chromosome type.</typeparam>
    /// <param name="keyPrefix">Key prefix used to build the operator keys.</param>
    /// <returns>Descriptors for the generational and island-model algorithms.</returns>
    /// <exception cref="ArgumentException"><paramref name="keyPrefix"/> is null or whitespace.</exception>
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