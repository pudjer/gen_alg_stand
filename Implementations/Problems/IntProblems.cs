using Algorithms.Chromosomes;
using Core.Contracts;
using Implementations.Common;
using Implementations.Modules;
using Parameters;
using Setup;

namespace Implementations.Problems;

public sealed class SumTargetProblemFactory : IProblemFactory<IntChromosome>
{
    private static readonly Parameter<int> Length = Params.Int("length", 1);
    private static readonly Parameter<int> MinValue = Params.Int("minValue");
    private static readonly Parameter<int> MaxValue = Params.Int("maxValue");
    private static readonly Parameter<int> Target = Params.Int("target");

    public IReadOnlyList<IParameter> Parameters { get; } = [Length, MinValue, MaxValue, Target];

    public IReadOnlyList<Variant<IntChromosome>> Create(ParameterSet values)
    {
        ArgumentNullException.ThrowIfNull(values);

        int length = values.Read(Length, 10);
        int minValue = values.Read(MinValue, 0);
        int maxValue = values.Read(MaxValue, 10);
        int target = values.Read(Target, 50);

        if (minValue > maxValue)
        {
            throw new ArgumentException("minValue must be less than or equal to maxValue.", nameof(values));
        }

        return new ProblemFactory<IntChromosome>
        {
            Algorithms = ModuleCatalog.Algorithms<IntChromosome>(),
            LossFunctions = [new SumTargetLossFactory<IntChromosome>(target)],
            Selections = ModuleCatalog.Selections<IntChromosome>(),
            Crossovers = ModuleCatalog.IntCrossovers(),
            Mutations = ModuleCatalog.IntMutations(),
            InitialPopulations = [new RandomIntPopulationFactory(length, minValue, maxValue)],
        }.Create(values);
    }
}