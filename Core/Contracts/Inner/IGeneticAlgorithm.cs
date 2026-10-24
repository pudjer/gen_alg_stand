using Core.Contracts.Outer;

namespace Core.Contracts.Inner;

/// <summary>
/// Внутренний API движка: единственный тип, который нужен вызывающему коду,
/// чтобы запустить алгоритм.
/// </summary>
public interface IGeneticAlgorithm
{
    IGeneticAlgorithmResult Run(
        IGeneticProblem problem,
        CancellationToken cancellationToken = default);
}
