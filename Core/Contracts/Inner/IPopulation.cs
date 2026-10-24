namespace Core.Contracts.Inner;

/// <summary>
/// Популяция одного поколения: хромосомы конкретного типа вместе с их
/// потерями. Заменяет прежнюю пару «особь = хромосома + приспособленность»:
/// оценка живёт рядом с хромосомой, но не внутри неё.
/// <para>
/// Inner-контракт: создаётся движком после оценки хромосом,
/// реализовывать его не нужно.
/// </para>
/// </summary>
/// <typeparam name="TChromosome">Конкретный тип хромосомы задачи.</typeparam>
public interface IPopulation<out TChromosome> : IReadOnlyList<TChromosome>
    where TChromosome : Outer.IChromosome
{
    /// <summary>
    /// Потери хромосомы по индексу: меньше — лучше. Оценки выровнены
    /// с порядком перечисления: длина совпадает с <see cref="IReadOnlyCollection{T}.Count"/>.
    /// </summary>
    double GetLoss(int index);
}
