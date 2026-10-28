using System.Globalization;
using Algorithms.Chromosomes;
using Implementations.Problems;

namespace GenAlgStand.Web.Catalog;

public sealed class ProblemCatalog
{
    public ProblemCatalog()
    {
        var problems = new List<IProblemDescriptor>
        {
            new ProblemDescriptor<BinaryChromosome>(
                "onemax", "OneMax", "Binary",
                new OneMaxProblemFactory(), DescribeBinary),
            new ProblemDescriptor<BinaryChromosome>(
                "longest-run", "Longest Run", "Binary",
                new LongestRunProblemFactory(), DescribeBinary),
            new ProblemDescriptor<BinaryChromosome>(
                "knapsack", "Knapsack", "Binary",
                new KnapsackProblemFactory(), DescribeBinary),

            new ProblemDescriptor<FloatChromosome>(
                "sphere", "Sphere", "Float",
                new SphereProblemFactory(), DescribeFloat),
            new ProblemDescriptor<FloatChromosome>(
                "rastrigin", "Rastrigin", "Float",
                new RastriginProblemFactory(), DescribeFloat),
            new ProblemDescriptor<FloatChromosome>(
                "rosenbrock", "Rosenbrock", "Float",
                new RosenbrockProblemFactory(), DescribeFloat),
            new ProblemDescriptor<FloatChromosome>(
                "ackley", "Ackley", "Float",
                new AckleyProblemFactory(), DescribeFloat),
            new ProblemDescriptor<FloatChromosome>(
                "griewank", "Griewank", "Float",
                new GriewankProblemFactory(), DescribeFloat),
            new ProblemDescriptor<FloatChromosome>(
                "schwefel", "Schwefel", "Float",
                new SchwefelProblemFactory(), DescribeFloat),
            new ProblemDescriptor<FloatChromosome>(
                "booth", "Booth", "Float",
                new BoothProblemFactory(), DescribeFloat),
            new ProblemDescriptor<FloatChromosome>(
                "beale", "Beale", "Float",
                new BealeProblemFactory(), DescribeFloat),
            new ProblemDescriptor<FloatChromosome>(
                "himmelblau", "Himmelblau", "Float",
                new HimmelblauProblemFactory(), DescribeFloat),
            new ProblemDescriptor<FloatChromosome>(
                "easom", "Easom", "Float",
                new EasomProblemFactory(), DescribeFloat),
            new ProblemDescriptor<FloatChromosome>(
                "bukin", "Bukin", "Float",
                new BukinProblemFactory(), DescribeFloat),
            new ProblemDescriptor<FloatChromosome>(
                "matyas", "Matyas", "Float",
                new MatyasProblemFactory(), DescribeFloat),
            new ProblemDescriptor<FloatChromosome>(
                "target-vector", "Target Vector", "Float",
                new TargetVectorProblemFactory(), DescribeFloat),

            new ProblemDescriptor<PermutationChromosome>(
                "tsp", "TSP", "Permutation",
                new TspProblemFactory(), DescribePermutation),
            new ProblemDescriptor<PermutationChromosome>(
                "n-queens", "N Queens", "Permutation",
                new NQueensProblemFactory(), DescribePermutation),
            new ProblemDescriptor<PermutationChromosome>(
                "inversion-count", "Inversion Count", "Permutation",
                new InversionCountProblemFactory(), DescribePermutation),

            new ProblemDescriptor<IntChromosome>(
                "sum-target", "Sum Target", "Int",
                new SumTargetProblemFactory(), DescribeInt),
        };

        All = problems;
        ById = problems.ToDictionary(problem => problem.Id, StringComparer.Ordinal);
    }

    public IReadOnlyList<IProblemDescriptor> All { get; }

    private IReadOnlyDictionary<string, IProblemDescriptor> ById { get; }

    public IProblemDescriptor? Find(string? id) =>
        id is not null && ById.TryGetValue(id, out IProblemDescriptor? problem) ? problem : null;

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