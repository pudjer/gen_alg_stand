using Core.Contracts.Inner;

namespace Core.Contracts.Outer;

/// <summary>
/// Целевая функция задачи.
/// </summary>
public interface IFitnessFunction
{
    /// <summary>
    /// Направление оптимизации. Задаётся функцией, а не прогоном:
    /// «минимизировать ошибку» — свойство задачи.
    /// </summary>
    bool IsMaximization { get; }

    double Evaluate(IChromosome chromosome, IExecutionContext context);
}
