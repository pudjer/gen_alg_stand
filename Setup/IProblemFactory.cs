using Core.Contracts;
using Parameters;

namespace Setup;

public interface IProblemFactory<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    IReadOnlyList<IParameter> Parameters { get; }

    IReadOnlyList<Variant<TChromosome>> Create(ParameterSet values);
}
