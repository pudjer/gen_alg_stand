using Algorithms.Chromosomes;
using Algorithms.Mutations;
using Parameters;

namespace Catalog.Descriptors;

/// <summary>Descriptors for mutation operators.</summary>
/// <remarks>
/// Mutations are generic over the chromosome type, so every method here is closed over
/// <c>TSelf</c> by the caller and returns ready-to-use descriptors.
/// </remarks>
public static class MutationDescriptors
{
    /// <summary>Gets the probability that a single gene is mutated.</summary>
    private static ParameterSpec GeneProbability { get; } =
        new("geneProbability", "Gene mutation probability", ParameterKind.Double, "0.01", Min: 0, Max: 1, Step: 0.01);

    /// <summary>Gets the probability that a chromosome is mutated.</summary>
    private static ParameterSpec ChromosomeProbability { get; } =
        new("chromosomeProbability", "Chromosome mutation probability", ParameterKind.Double, "0.1", Min: 0, Max: 1, Step: 0.05);

    /// <summary>Gets the mutations specific to binary chromosomes.</summary>
    /// <typeparam name="TSelf">Concrete binary chromosome type.</typeparam>
    /// <param name="keyPrefix">Key prefix used to build the operator keys.</param>
    /// <returns>The descriptor for bit-flip mutation.</returns>
    /// <exception cref="ArgumentException"><paramref name="keyPrefix"/> is null or whitespace.</exception>
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

    /// <summary>Gets the mutations specific to floating-point chromosomes.</summary>
    /// <typeparam name="TSelf">Concrete floating-point chromosome type.</typeparam>
    /// <param name="keyPrefix">Key prefix used to build the operator keys.</param>
    /// <returns>Descriptors for Gaussian and uniform float mutation.</returns>
    /// <exception cref="ArgumentException"><paramref name="keyPrefix"/> is null or whitespace.</exception>
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

    /// <summary>Gets the mutations specific to integer chromosomes.</summary>
    /// <typeparam name="TSelf">Concrete integer chromosome type.</typeparam>
    /// <param name="keyPrefix">Key prefix used to build the operator keys.</param>
    /// <returns>The descriptor for uniform integer mutation.</returns>
    /// <exception cref="ArgumentException"><paramref name="keyPrefix"/> is null or whitespace.</exception>
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

    /// <summary>Gets the mutations specific to ordered chromosomes.</summary>
    /// <typeparam name="TSelf">Concrete ordered chromosome type.</typeparam>
    /// <param name="keyPrefix">Key prefix used to build the operator keys.</param>
    /// <returns>Descriptors for swap, insert, inversion and scramble mutation.</returns>
    /// <exception cref="ArgumentException"><paramref name="keyPrefix"/> is null or whitespace.</exception>
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
