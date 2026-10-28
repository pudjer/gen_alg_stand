using Core.Contracts;

namespace GenAlgStand.Web.Catalog;

public sealed record Visualization(string Kind, string Title, string Json);

public sealed record VisualizationContext<TChromosome>(
    TChromosome Best,
    ILossFunction<TChromosome> Loss,
    IReadOnlyList<Sample> Samples)
    where TChromosome : IChromosome<TChromosome>;

public interface IProblemVisualizer<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    Visualization? Build(VisualizationContext<TChromosome> context);
}