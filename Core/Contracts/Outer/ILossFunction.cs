namespace Core.Contracts;

public interface ILossFunction<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    double Evaluate(TChromosome chromosome);
}
