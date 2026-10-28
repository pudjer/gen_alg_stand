namespace Core.Contracts;


public interface IRandom
{
    double NextDouble();
    int Next(int minInclusive, int maxExclusive);
}


