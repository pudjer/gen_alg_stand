using Parameters;
using Studio;

namespace Catalog;

public interface ICatalog
{
    IReadOnlyList<IProblemEntry> Problems { get; }

    IProblemEntry? FindProblem(string key);

    ExperimentConfig CreateDefaultConfig(string problemKey);
}
