using Core.Contracts.Inner;

namespace Core.Contracts.Outer;

/// <summary>
/// Стратегия мутации.
/// </summary>
/// <typeparam name="TChromosome">Конкретный тип хромосомы задачи.</typeparam>
public interface IMutation<TChromosome>
    where TChromosome : IChromosome
{
    /// <summary>
    /// Мутирует хромосому. Случайность берётся из общего источника прогона:
    /// свой Random заводить не надо.
    /// </summary>
    TChromosome Mutate(TChromosome chromosome, IRandom random);
}
