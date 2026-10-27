using Core.Contracts;

namespace Algorithms.Random;

/// <summary>
/// A deterministic <see cref="IRandom"/> whose sequence depends only on the seed, so two
/// instances created with the same seed produce the same numbers.
/// </summary>
/// <remarks>
/// The generator is a small xorshift-based stream. It is <b>not</b> thread-safe: create one
/// instance per worker when running in parallel.
/// </remarks>
public sealed class SeededRandom : IRandom
{
    private uint _state;

    /// <summary>
    /// Initializes a new instance of the <see cref="SeededRandom"/> class.
    /// </summary>
    /// <param name="seed">The seed; the same seed always yields the same sequence.</param>
    public SeededRandom(int seed)
    {
        // Zero is a fixed point of xorshift, so map it to a non-zero constant.
        _state = seed == 0 ? 0x9E3779B9u : unchecked((uint)seed);
    }

    /// <summary>Creates a generator seeded from the current system clock.</summary>
    /// <returns>A new <see cref="SeededRandom"/>.</returns>
    public static SeededRandom FromClock() => new(Environment.TickCount);

    /// <inheritdoc />
    public double NextDouble() => NextUInt32() / 4294967296.0;

    /// <inheritdoc />
    public int Next(int minInclusive, int maxExclusive)
    {
        if (maxExclusive <= minInclusive)
        {
            return minInclusive;
        }

        ulong range = (ulong)((long)maxExclusive - minInclusive);
        return (int)(minInclusive + (long)(NextUInt32() % range));
    }

    private uint NextUInt32()
    {
        uint x = _state;
        x ^= x << 13;
        x ^= x >> 17;
        x ^= x << 5;
        _state = x;
        return x;
    }
}
