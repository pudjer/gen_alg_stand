using Core.Contracts.Inner;

namespace Core.Contracts.Outer;

/// <summary>
/// Поставщик начальной популяции — точка входа в пространство решений.
/// <para>
/// Outer-контракт: реализуется вне движка. Размер популяции и способ
/// генерации хромосом — решение задачи, движок их не задаёт и не проверяет.
/// </para>
/// </summary>
public interface IInitialPopulation
{
    /// <summary>
    /// Создаёт хромосомы первого поколения. Оценку приспособленности
    /// выполняет движок — возвращать её не нужно.
    /// </summary>
    IReadOnlyList<IChromosome> Create(IExecutionContext context);
}
