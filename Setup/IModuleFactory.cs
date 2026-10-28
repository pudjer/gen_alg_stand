using Parameters;

namespace Setup;

public interface IModuleFactory<out TModule>
{
    string Name { get; }

    IReadOnlyList<IParameter> Parameters { get; }

    TModule Create(ParameterSet values);
}
