namespace Core.Contracts;


public interface IRandom
{
    double NextDouble();
    int Next(int minInclusive, int maxExclusive);
}

public sealed class SameSeedRandom
{
    private readonly Random _random;

    public SameSeedRandom()
    {
        _random = new Random(1);
    }


    public double NextDouble() => _random.NextDouble();

    public int Next(int minInclusive, int maxExclusive)
        => _random.Next(minInclusive, maxExclusive);
}