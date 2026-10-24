using Core.Contracts.Inner;

namespace Core.Contracts.Outer;

/// <summary>
/// Поставщик начальной популяции — точка входа в пространство решений.
/// <para>
/// Outer-контракт: реализуется вне движка. Размер популяции и способ
/// генерации хромосом — решение задачи, движок их не задаёт и не проверяет.
/// </para>
/// </summary>
/// <typeparam name="TChromosome">Конкретный тип хромосомы задачи.</typeparam>
public interface IInitialPopulation<TChromosome>
    where TChromosome : IChromosome
{
    /// <summary>
    /// Создаёт хромосомы первого поколения. Оценку потерь
    /// выполняет движок — возвращать её не нужно. Случайность берётся
    /// из общего источника прогона: свой Random заводить не надо.
    /// </summary>
    IReadOnlyList<TChromosome> Create(IRandom random);
}
