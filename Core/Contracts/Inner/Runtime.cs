namespace Core.Contracts;


public interface IRuntime<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    IObserver<TChromosome> Observer { get; }
    IRandomFactory RandomFactory { get; }
}

public struct Runtime<TChromosome> : IRuntime<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    public required IObserver<TChromosome> Observer { get; init; }
    public required IRandomFactory RandomFactory { get; init; }


    public Runtime()
    {
        Observer = new SubscribableObserver<TChromosome>();
        RandomFactory = new SeededRandomFactory(44);
    }
}