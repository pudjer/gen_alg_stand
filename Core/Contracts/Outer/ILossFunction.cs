namespace Core.Contracts;

public interface ILossFunction<TChromosome>
    where TChromosome : IChromosome
{
    double Evaluate(TChromosome chromosome);
}
