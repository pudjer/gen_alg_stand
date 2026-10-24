using Core.Contracts.Inner;

namespace Core.Contracts.Outer;

/// <summary>
/// Стратегия скрещивания.
/// </summary>
public interface ICrossover
{
    (IChromosome First, IChromosome Second) Crossover(
        IChromosome first,
        IChromosome second,
        IExecutionContext context);
}
