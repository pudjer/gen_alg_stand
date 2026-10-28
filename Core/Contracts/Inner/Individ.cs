namespace Core.Contracts;

public sealed class Individ<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    private readonly ILossFunction<TChromosome> _lossFunction;
    private double? _lossValue;

    public Individ(TChromosome chromosome, ILossFunction<TChromosome> lossFunction)
    {
        ArgumentNullException.ThrowIfNull(chromosome);
        ArgumentNullException.ThrowIfNull(lossFunction);

        Chromosome = chromosome;
        _lossFunction = lossFunction;
    }

    public TChromosome Chromosome { get; init; }

    public double LossValue
    {
        get
        {
            double? cached = _lossValue;
            if (cached is null)
            {
                cached = _lossFunction.Evaluate(Chromosome);
                _lossValue = cached;
            }

            return cached.GetValueOrDefault();
        }
    }

}
