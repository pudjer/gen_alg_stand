using Core.Contracts;
using Parameters;

namespace Setup;

public sealed class Variant<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    public required IModuleFactory<ILossFunction<TChromosome>> LossFunction { get; init; }

    public required IModuleFactory<ISelection<TChromosome>> Selection { get; init; }

    public required IModuleFactory<ICrossover<TChromosome>> Crossover { get; init; }

    public required IModuleFactory<IMutation<TChromosome>> Mutation { get; init; }

    public required IModuleFactory<IInitialPopulation<TChromosome>> InitialPopulation { get; init; }


    public GeneticProblem<TChromosome> Create(
        ParameterSet lossFunction,
        ParameterSet selection,
        ParameterSet crossover,
        ParameterSet mutation,
        ParameterSet initialPopulation) =>
        new()
        {
            LossFunction = LossFunction.Create(lossFunction),
            Selection = Selection.Create(selection),
            Crossover = Crossover.Create(crossover),
            Mutation = Mutation.Create(mutation),
            InitialPopulation = InitialPopulation.Create(initialPopulation),
        };

    public override string ToString() =>
        $"{LossFunction.Name} / {Selection.Name} / {Crossover.Name} / {Mutation.Name} / " +
        $"{InitialPopulation.Name}";
}
