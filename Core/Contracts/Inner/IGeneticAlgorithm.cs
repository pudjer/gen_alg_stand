using Core.Contracts.Outer;

namespace Core.Contracts.Inner;

/// <summary>
/// Внутренний API движка: единственный тип, который нужен вызывающему коду,
/// чтобы запустить алгоритм.
/// </summary>
/// <typeparam name="TChromosome">Конкретный тип хромосомы задачи.</typeparam>
public interface IGeneticAlgorithm<TChromosome>
    where TChromosome : IChromosome
{
    /// <summary>Запускает прогон над задачей с тем же типом хромосомы.</summary>
    IGeneticAlgorithmResult<TChromosome> Run(
        IGeneticProblem<TChromosome> problem,
        CancellationToken cancellationToken = default);
}
