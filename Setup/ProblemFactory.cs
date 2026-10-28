using Core.Contracts;
using Parameters;

namespace Setup;

public sealed class ProblemFactory<TChromosome> : IProblemFactory<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    public required IReadOnlyList<IModuleFactory<ILossFunction<TChromosome>>> LossFunctions { get; init; }

    public required IReadOnlyList<IModuleFactory<ISelection<TChromosome>>> Selections { get; init; }

    public required IReadOnlyList<IModuleFactory<ICrossover<TChromosome>>> Crossovers { get; init; }

    public required IReadOnlyList<IModuleFactory<IMutation<TChromosome>>> Mutations { get; init; }

    public required IReadOnlyList<IModuleFactory<IInitialPopulation<TChromosome>>> InitialPopulations { get; init; }

    public required IReadOnlyList<IModuleFactory<Core.Contracts.IObserver<TChromosome>>> Observers { get; init; }

    public IReadOnlyList<IParameter> Parameters { get; init; } = [];

    public IReadOnlyList<Variant<TChromosome>> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);

        var variants = new List<Variant<TChromosome>>(
            LossFunctions.Count * Selections.Count * Crossovers.Count *
            Mutations.Count * InitialPopulations.Count * Observers.Count);

        foreach (var loss in LossFunctions)
        foreach (var selection in Selections)
        foreach (var crossover in Crossovers)
        foreach (var mutation in Mutations)
        foreach (var initialPopulation in InitialPopulations)
        foreach (var observer in Observers)
        {
            variants.Add(new Variant<TChromosome>
            {
                LossFunction = loss,
                Selection = selection,
                Crossover = crossover,
                Mutation = mutation,
                InitialPopulation = initialPopulation,
                Observer = observer,
            });
        }

        return variants;
    }
}
