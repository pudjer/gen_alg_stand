using Core.Contracts;

namespace Algorithms.Random;

public sealed class SeededRandomFactory : IRandomFactory
{
    private int _issued;

    public SeededRandomFactory(int seed) => Seed = seed;

    public int Seed { get; }

    public IRandom Create() => new SeededRandom(Mix(Seed, Interlocked.Increment(ref _issued) - 1));

    private static int Mix(int seed, int ordinal)
    {
        uint x = unchecked((uint)seed) + (0x9E3779B9u * (uint)(ordinal + 1));
        x ^= x >> 16;
        x *= 0x7FEB352Du;
        x ^= x >> 15;
        x *= 0x846CA68Bu;
        x ^= x >> 16;

        return unchecked((int)x);
    }
}
