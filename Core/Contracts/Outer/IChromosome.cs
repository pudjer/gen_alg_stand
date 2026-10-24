namespace Core.Contracts.Outer;

/// <summary>
/// Решение задачи — последовательность генов.
/// </summary>
public interface IChromosome
{
    IReadOnlyList<IGene> Genes { get; }

    IChromosome Clone();
}
