namespace Core.Contracts;

public interface IChromosome<TSelf> where TSelf : IChromosome<TSelf>
{
    TSelf Clone();
}