using Algorithms.Chromosomes;
using Algorithms.Mutations;
using Parameters;

namespace Catalog.Descriptors;

public static class MutationDescriptors
{
    private static ParameterSpec GeneProbability { get; } =
        new("geneProbability", "Gene mutation probability", ParameterKind.Double, "0.01", Min: 0, Max: 1, Step: 0.01);

    private static ParameterSpec ChromosomeProbability { get; } =
        new("chromosomeProbability", "Chromosome mutation probability", ParameterKind.Double, "0.1", Min: 0, Max: 1, Step: 0.05);

    public static IReadOnlyList<OperatorDescriptor> ForBinary<TSelf>(string keyPrefix = "mutation")
        where TSelf : IBinaryChromosome<TSelf>
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(keyPrefix);

        return
        [
            new($"{keyPrefix}.bitflip", "Bit-flip mutation", OperatorDescriptor.Specs(GeneProbability, ChromosomeProbability),
                p => new BitFlipMutation<TSelf>(p.GetDouble("geneProbability"), p.GetDouble("chromosomeProbability"))),
        ];
    }

    public static IReadOnlyList<OperatorDescriptor> ForFloat<TSelf>(string keyPrefix = "mutation")
        where TSelf : IFloatChromosome<TSelf>
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(keyPrefix);

        return
        [
            new($"{keyPrefix}.gaussian", "Gaussian mutation", OperatorDescriptor.Specs(
                    GeneProbability,
                    new ParameterSpec("sigma", "Noise standard deviation", ParameterKind.Double, "0.1", Min: 0, Max: 2, Step: 0.05),
                    ChromosomeProbability),
                p => new GaussianMutation<TSelf>(
                    p.GetDouble("geneProbability"),
                    p.GetDouble("sigma"),
                    p.GetDouble("chromosomeProbability"))),
            new($"{keyPrefix}.uniformfloat", "Uniform float mutation", OperatorDescriptor.Specs(
                    GeneProbability,
                    ChromosomeProbability),
                p => new UniformFloatMutation<TSelf>(p.GetDouble("geneProbability"), p.GetDouble("chromosomeProbability"))),
        ];
    }

    public static IReadOnlyList<OperatorDescriptor> ForInt<TSelf>(string keyPrefix = "mutation")
        where TSelf : IIntChromosome<TSelf>
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(keyPrefix);

        return
        [
            new($"{keyPrefix}.uniformint", "Uniform integer mutation", OperatorDescriptor.Specs(
                    GeneProbability,
                    ChromosomeProbability),
                p => new UniformIntMutation<TSelf>(p.GetDouble("geneProbability"), p.GetDouble("chromosomeProbability"))),
        ];
    }

    public static IReadOnlyList<OperatorDescriptor> ForOrdered<TSelf>(string keyPrefix = "mutation")
        where TSelf : IOrderedChromosome<TSelf>
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(keyPrefix);

        return
        [
            new($"{keyPrefix}.swap", "Swap mutation", OperatorDescriptor.Specs(ChromosomeProbability),
                p => new SwapMutation<TSelf>(p.GetDouble("chromosomeProbability"))),
            new($"{keyPrefix}.insert", "Insert mutation", OperatorDescriptor.Specs(ChromosomeProbability),
                p => new InsertMutation<TSelf>(p.GetDouble("chromosomeProbability"))),
            new($"{keyPrefix}.inversion", "Inversion mutation", OperatorDescriptor.Specs(ChromosomeProbability),
                p => new InversionMutation<TSelf>(p.GetDouble("chromosomeProbability"))),
            new($"{keyPrefix}.scramble", "Scramble mutation", OperatorDescriptor.Specs(ChromosomeProbability),
                p => new ScrambleMutation<TSelf>(p.GetDouble("chromosomeProbability"))),
        ];
    }
}
