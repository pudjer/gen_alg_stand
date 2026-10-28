using Core.Contracts;
using Parameters;
using Setup;

namespace GenAlgStand.Web.Catalog;

public interface IProblemDescriptor
{
    string Id { get; }

    string Name { get; }

    string Family { get; }

    IReadOnlyList<IParameter> Parameters { get; }

    IRunConfiguration Configure(ParameterSet values);
}

public sealed class ProblemDescriptor<TChromosome> : IProblemDescriptor
    where TChromosome : IChromosome<TChromosome>
{
    private readonly IProblemFactory<TChromosome> _factory;
    private readonly Func<TChromosome, string> _describe;

    public ProblemDescriptor(
        string id,
        string name,
        string family,
        IProblemFactory<TChromosome> factory,
        Func<TChromosome, string> describe)
    {
        Id = id;
        Name = name;
        Family = family;
        _factory = factory;
        _describe = describe;
    }

    public string Id { get; }

    public string Name { get; }

    public string Family { get; }

    public IReadOnlyList<IParameter> Parameters => _factory.Parameters;

    public IRunConfiguration Configure(ParameterSet values) =>
        new RunConfiguration<TChromosome>(_factory.Create(values), _describe);
}