using System.Globalization;
using Algorithms.Chromosomes;
using GenAlgStand.Web.Catalog.Visualizers;
using Implementations.Problems;
using Setup;

namespace GenAlgStand.Web.Catalog;

public sealed class ProblemCatalog
{
    private static readonly IProblemVisualizer<BinaryChromosome> BinaryVisualizer = new BinaryBitsVisualizer();

    private static readonly IProblemVisualizer<FloatChromosome> FloatVisualizer =
        new NumericVisualizer<FloatChromosome>(
            chromosome => chromosome.Genes,
            chromosome => (chromosome.MinValue, chromosome.MaxValue),
            genes => new FloatChromosome(genes, double.MinValue, double.MaxValue),
            integer: false);

    private static readonly IProblemVisualizer<IntChromosome> IntVisualizer =
        new NumericVisualizer<IntChromosome>(
            chromosome => chromosome.Genes.Select(gene => (double)gene).ToArray(),
            chromosome => (chromosome.MinValue, chromosome.MaxValue),
            genes => new IntChromosome(
                genes.Select(gene => (int)Math.Round(gene, MidpointRounding.AwayFromZero)).ToArray(),
                int.MinValue,
                int.MaxValue),
            integer: true);

    private static readonly IProblemVisualizer<PermutationChromosome> TspVisualizer =
        new TspMapVisualizer();

    private static readonly IProblemVisualizer<PermutationChromosome> QueensVisualizer =
        new NQueensVisualizer();

    public ProblemCatalog()
    {
        var problems = new List<IProblemDescriptor>
        {
            Binary("onemax", "OneMax", new OneMaxProblemFactory()),
            Binary("longest-run", "Longest Run", new LongestRunProblemFactory()),
            Binary("knapsack", "Knapsack", new KnapsackProblemFactory()),

            Float("sphere", "Sphere", new SphereProblemFactory()),
            Float("rastrigin", "Rastrigin", new RastriginProblemFactory()),
            Float("rosenbrock", "Rosenbrock", new RosenbrockProblemFactory()),
            Float("ackley", "Ackley", new AckleyProblemFactory()),
            Float("griewank", "Griewank", new GriewankProblemFactory()),
            Float("schwefel", "Schwefel", new SchwefelProblemFactory()),
            Float("booth", "Booth", new BoothProblemFactory()),
            Float("beale", "Beale", new BealeProblemFactory()),
            Float("himmelblau", "Himmelblau", new HimmelblauProblemFactory()),
            Float("easom", "Easom", new EasomProblemFactory()),
            Float("bukin", "Bukin", new BukinProblemFactory()),
            Float("matyas", "Matyas", new MatyasProblemFactory()),
            Float("target-vector", "Target Vector", new TargetVectorProblemFactory()),

            Permutation("tsp", "TSP", new TspProblemFactory(), TspVisualizer),
            Permutation("n-queens", "N Queens", new NQueensProblemFactory(), QueensVisualizer),
            Permutation("inversion-count", "Inversion Count", new InversionCountProblemFactory(), null),

            Int("sum-target", "Sum Target", new SumTargetProblemFactory()),
        };

        All = problems;
        ById = problems.ToDictionary(problem => problem.Id, StringComparer.Ordinal);
    }

    public IReadOnlyList<IProblemDescriptor> All { get; }

    private IReadOnlyDictionary<string, IProblemDescriptor> ById { get; }

    public IProblemDescriptor? Find(string? id) =>
        id is not null && ById.TryGetValue(id, out IProblemDescriptor? problem) ? problem : null;

    private static ProblemDescriptor<BinaryChromosome> Binary(
        string id,
        string name,
        IProblemFactory<BinaryChromosome> factory) =>
        new(id, name, "Binary", factory, DescribeBinary, BinaryVisualizer);

    private static ProblemDescriptor<FloatChromosome> Float(
        string id,
        string name,
        IProblemFactory<FloatChromosome> factory) =>
        new(id, name, "Float", factory, DescribeFloat, FloatVisualizer);

    private static ProblemDescriptor<IntChromosome> Int(
        string id,
        string name,
        IProblemFactory<IntChromosome> factory) =>
        new(id, name, "Int", factory, DescribeInt, IntVisualizer);

    private static ProblemDescriptor<PermutationChromosome> Permutation(
        string id,
        string name,
        IProblemFactory<PermutationChromosome> factory,
        IProblemVisualizer<PermutationChromosome>? visualizer) =>
        new(id, name, "Permutation", factory, DescribePermutation, visualizer);

    private static string DescribeBinary(BinaryChromosome chromosome) =>
        string.Concat(Enumerable.Range(0, chromosome.Length).Select(index => chromosome[index] ? '1' : '0'));

    private static string DescribeFloat(FloatChromosome chromosome) =>
        string.Join(
            " ",
            chromosome.Genes.Select(gene => gene.ToString("0.####", CultureInfo.InvariantCulture)));

    private static string DescribeInt(IntChromosome chromosome) =>
        string.Join(" ", chromosome.Genes.Select(gene => gene.ToString(CultureInfo.InvariantCulture)));

    private static string DescribePermutation(PermutationChromosome chromosome) =>
        string.Join(" ", chromosome.Genes.Select(gene => gene.ToString(CultureInfo.InvariantCulture)));
}