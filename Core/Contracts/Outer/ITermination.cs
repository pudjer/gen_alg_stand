using Core.Contracts.Inner;

namespace Core.Contracts.Outer;

/// <summary>
/// Критерий останова задачи — единственный, который пишет пользователь.
/// <para>
/// Отмена проверяется движком самостоятельно, дублировать её здесь не нужно.
/// </para>
/// </summary>
/// <typeparam name="TChromosome">Конкретный тип хромосомы задачи.</typeparam>
public interface ITermination<TChromosome>
    where TChromosome : IChromosome
{
    /// <summary>
    /// Вызывается на границе поколения, до создания следующего.
    /// <c>true</c> — завершить прогон.
    /// </summary>
    bool ShouldStop(IExecutionContext<TChromosome> context);
}
