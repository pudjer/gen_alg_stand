using Algorithms.Chromosomes;
using Algorithms.Crossovers;
using Parameters;

namespace Catalog.Descriptors;

public static class CrossoverDescriptors
{
    private static ParameterSpec Probability { get; } =
        new("crossoverProbability", "Crossover probability", ParameterKind.Double, "0.9", Min: 0, Max: 1, Step: 0.05);

    public static IReadOnlyList<OperatorDescriptor> For<TSelf, TGene>(string keyPrefix = "crossover")
        where TSelf : IGenotypeChromosome<TSelf, TGene>
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(keyPrefix);

        return
        [
            new($"{keyPrefix}.singlepoint", "Single-point crossover", OperatorDescriptor.Specs(Probability),
                p => new SinglePointCrossover<TSelf, TGene>(p.GetDouble("crossoverProbability"))),
            new($"{keyPrefix}.twopoint", "Two-point crossover", OperatorDescriptor.Specs(Probability),
                p => new TwoPointCrossover<TSelf, TGene>(p.GetDouble("crossoverProbability"))),
            new($"{keyPrefix}.uniform", "Uniform crossover", OperatorDescriptor.Specs(
                    new ParameterSpec("swapProbability", "Gene swap probability", ParameterKind.Double, "0.5", Min: 0, Max: 1, Step: 0.05),
                    Probability),
                p => new UniformCrossover<TSelf, TGene>(p.GetDouble("swapProbability"), p.GetDouble("crossoverProbability"))),
        ];
    }

    public static IReadOnlyList<OperatorDescriptor> ForFloat<TSelf>(string keyPrefix = "crossover")
        where TSelf : IFloatChromosome<TSelf>
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(keyPrefix);

        return
        [
            new($"{keyPrefix}.arithmetic", "Arithmetic crossover", OperatorDescriptor.Specs(
                    new ParameterSpec("weight", "Blend weight", ParameterKind.Double, "0.5", Min: 0, Max: 1, Step: 0.05),
                    Probability),
                p => new ArithmeticCrossover<TSelf>(p.GetDouble("weight"), p.GetDouble("crossoverProbability"))),
            new($"{keyPrefix}.blend", "Blend crossover", OperatorDescriptor.Specs(
                    new ParameterSpec("alpha", "Blend interval extension", ParameterKind.Double, "0.5", Min: 0, Max: 2, Step: 0.1),
                    Probability),
                p => new BlendCrossover<TSelf>(p.GetDouble("alpha"), p.GetDouble("crossoverProbability"))),
        ];
    }

    public static IReadOnlyList<OperatorDescriptor> ForInt<TSelf>(string keyPrefix = "crossover")
        where TSelf : IIntChromosome<TSelf>
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(keyPrefix);

        return
        [
            new($"{keyPrefix}.integerarithmetic", "Integer arithmetic crossover", OperatorDescriptor.Specs(
                    new ParameterSpec("weight", "Blend weight", ParameterKind.Double, "0.5", Min: 0, Max: 1, Step: 0.05),
                    Probability),
                p => new IntegerArithmeticCrossover<TSelf>(p.GetDouble("weight"), p.GetDouble("crossoverProbability"))),
        ];
    }

    public static IReadOnlyList<OperatorDescriptor> ForOrdered<TSelf>(string keyPrefix = "crossover")
        where TSelf : IOrderedChromosome<TSelf>
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(keyPrefix);

        return
        [
            new($"{keyPrefix}.cycle", "Cycle crossover", OperatorDescriptor.Specs(Probability),
                p => new CycleCrossover<TSelf>(p.GetDouble("crossoverProbability"))),
            new($"{keyPrefix}.order", "Order crossover", OperatorDescriptor.Specs(Probability),
                p => new OrderCrossover<TSelf>(p.GetDouble("crossoverProbability"))),
            new($"{keyPrefix}.partiallymapped", "Partially mapped crossover", OperatorDescriptor.Specs(Probability),
                p => new PartiallyMappedCrossover<TSelf>(p.GetDouble("crossoverProbability"))),
            new($"{keyPrefix}.orderbased", "Order-based crossover", OperatorDescriptor.Specs(Probability),
                p => new OrderBasedCrossover<TSelf>(p.GetDouble("crossoverProbability"))),
        ];
    }
}
