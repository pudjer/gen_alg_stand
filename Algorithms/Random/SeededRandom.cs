using Core.Contracts;

namespace Algorithms.Random;

public sealed class SeededRandom : IRandom
{
    private uint _state;

    public SeededRandom(int seed)
    {
        _state = seed == 0 ? 0x9E3779B9u : unchecked((uint)seed);
    }

    public double NextDouble() => NextUInt32() / 4294967296.0;

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
