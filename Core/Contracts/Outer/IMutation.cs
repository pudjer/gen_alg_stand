using Core.Contracts.Inner;

namespace Core.Contracts.Outer;

/// <summary>
/// Стратегия мутации.
/// </summary>
public interface IMutation
{
    IChromosome Mutate(IChromosome chromosome, IExecutionContext context);
}
