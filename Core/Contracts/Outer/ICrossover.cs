using Core.Contracts.Inner;

namespace Core.Contracts.Outer;

/// <summary>
/// Стратегия скрещивания. На вход и выход идёт тот же тип хромосомы,
/// что задан задачей — приведений и кастов нет.
/// </summary>
/// <typeparam name="TChromosome">Конкретный тип хромосомы задачи.</typeparam>
public interface ICrossover<TChromosome>
    where TChromosome : IChromosome
{
    (TChromosome First, TChromosome Second) Crossover(
        TChromosome first,
        TChromosome second,
        IRandom random);
}
