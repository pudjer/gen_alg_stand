using Algorithms.Chromosomes;
using Core.Contracts;

namespace Algorithms.LossFunctions;

/// <summary>
/// Minimization loss that rewards binary chromosomes containing many true genes.
/// </summary>
/// <typeparam name="TChromosome">The binary chromosome type being evaluated.</typeparam>
public sealed class OneMaxLossFunction<TChromosome> : ILossFunction<TChromosome>
    where TChromosome : IBinaryChromosome<TChromosome>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OneMaxLossFunction{TChromosome}"/> class.
    /// </summary>
    public OneMaxLossFunction()
    {
    }

    /// <summary>
    /// Returns the negated number of true genes, so a larger gene count yields a lower loss.
    /// </summary>
    /// <param name="chromosome">The binary chromosome to evaluate.</param>
    /// <returns>The negated count of true genes.</returns>
    public double Evaluate(TChromosome chromosome)
    {
        ArgumentNullException.ThrowIfNull(chromosome);

        int count = 0;
        for (int i = 0; i < chromosome.Length; i++)
        {
            if (chromosome[i])
            {
                count++;
            }
        }

        return -count;
    }
}
