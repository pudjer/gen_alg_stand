namespace Core.Contracts.Inner;

/// <summary>
/// Источник случайных чисел. Живёт в контексте, а не в стратегиях,
/// чтобы прогон был воспроизводим по одному seed.
/// <para>
/// Inner-контракт: предоставляется движком, реализовывать его не нужно.
/// </para>
/// </summary>
public interface IRandom
{
    double NextDouble();

    int Next(int minInclusive, int maxExclusive);
}
