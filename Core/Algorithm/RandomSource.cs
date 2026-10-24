namespace Core.Algorithm;

using Core.Contracts.Inner;

/// <summary>
/// Источник случайных чисел движка.
/// Единственное место, где создаётся случайность: стратегии получают
/// готовый <see cref="IRandom"/> через контекст и своих генераторов не заводят.
/// </summary>
internal sealed class RandomSource : IRandom
{
    private readonly Random _random = new();

    public double NextDouble() => _random.NextDouble();

    public int Next(int minInclusive, int maxExclusive)
        => _random.Next(minInclusive, maxExclusive);
}
