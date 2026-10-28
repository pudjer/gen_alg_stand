using Algorithms.Chromosomes;
using Algorithms.InitialPopulations;
using Parameters;

namespace Catalog.Descriptors;

public static class InitialPopulationDescriptors
{
    private static ParameterSpec PopulationSize { get; } =
        new("populationSize", "Population size", ParameterKind.Int, "50", Min: 2, Max: 10000, Step: 1);

    public static IReadOnlyList<OperatorDescriptor> ForBinary<TSelf>(TSelf template, string keyPrefix = "population")
        where TSelf : IBinaryChromosome<TSelf>
    {
        ArgumentNullException.ThrowIfNull(template);

        return
        [
            new($"{keyPrefix}.binary", "Random binary population", OperatorDescriptor.Specs(
                    PopulationSize,
                    new ParameterSpec("trueProbability", "Probability of a true gene", ParameterKind.Double, "0.5", Min: 0, Max: 1, Step: 0.05)),
                p => new RandomBinaryPopulation<TSelf>(p.GetInt("populationSize"), template, p.GetDouble("trueProbability"))),
        ];
    }

    public static IReadOnlyList<OperatorDescriptor> ForFloat<TSelf>(TSelf template, string keyPrefix = "population")
        where TSelf : IFloatChromosome<TSelf>
    {
        ArgumentNullException.ThrowIfNull(template);

        return
        [
            new($"{keyPrefix}.float", "Random float population", OperatorDescriptor.Specs(PopulationSize),
                p => new RandomFloatPopulation<TSelf>(p.GetInt("populationSize"), template)),
        ];
    }

    public static IReadOnlyList<OperatorDescriptor> ForInt<TSelf>(TSelf template, string keyPrefix = "population")
        where TSelf : IIntChromosome<TSelf>
    {
        ArgumentNullException.ThrowIfNull(template);

        return
        [
            new($"{keyPrefix}.int", "Random integer population", OperatorDescriptor.Specs(PopulationSize),
                p => new RandomIntPopulation<TSelf>(p.GetInt("populationSize"), template)),
        ];
    }

    public static IReadOnlyList<OperatorDescriptor> ForPermutation<TSelf>(TSelf template, string keyPrefix = "population")
        where TSelf : IPermutationChromosome<TSelf>
    {
        ArgumentNullException.ThrowIfNull(template);

        return
        [
            new($"{keyPrefix}.permutation", "Random permutation population", OperatorDescriptor.Specs(PopulationSize),
                p => new RandomPermutationPopulation<TSelf>(p.GetInt("populationSize"), template)),
        ];
    }
}