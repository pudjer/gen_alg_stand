using Core.Contracts;

namespace Algorithms.Orchestration;

public sealed class GenerationalAlgorithm<TChromosome> : IAlgorithm<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    private readonly GenerationalOptions _options;

    public GenerationalAlgorithm(GenerationalOptions? options = null)
    {
        GenerationalOptions resolved = options ?? GenerationalOptions.Default;
        ArgumentOutOfRangeException.ThrowIfNegative(resolved.MaxGenerations, nameof(options));
        ArgumentOutOfRangeException.ThrowIfNegative(resolved.EliteCount, nameof(options));

        _options = resolved;
    }

    public void Run(GeneticProblem<TChromosome> problem)
    {
        IRandom random = problem.RandomFactory.Create()
            ?? throw new InvalidOperationException("The random factory must not return null.");

        IReadOnlyList<TChromosome> population = problem.InitialPopulation.Create(random)
            ?? throw new InvalidOperationException("The initial population must not be null.");

        problem.Observer.OnGeneration(population);

        for (int generation = 0; generation < _options.MaxGenerations; generation++)
        {
            IReadOnlyList<TChromosome>? next = Step(problem, population, random);
            if (next is null)
            {
                return;
            }

            population = next;
            problem.Observer.OnGeneration(population);
        }
    }

    internal IReadOnlyList<TChromosome>? Step(
        GeneticProblem<TChromosome> problem,
        IReadOnlyList<TChromosome> population,
        IRandom random)
    {
        if (population.Count == 0)
        {
            return null;
        }

        IReadOnlyList<TChromosome> selected = problem.Selection.Select(population, random);
        IReadOnlyList<TChromosome> children = problem.Crossover.Crossover(selected, random);
        IReadOnlyList<TChromosome> mutated = problem.Mutation.Mutate(children, random);
        IReadOnlyList<TChromosome> next = ApplyElitism(problem, population, mutated);

        if (_options.MaintainPopulationSize && next.Count != population.Count)
        {
            next = Resize(next, population.Count, random);
        }

        return next;
    }

    private IReadOnlyList<TChromosome> ApplyElitism(
        GeneticProblem<TChromosome> problem,
        IReadOnlyList<TChromosome> parents,
        IReadOnlyList<TChromosome> offspring)
    {
        int eliteCount = Math.Min(_options.EliteCount, Math.Min(parents.Count, offspring.Count));
        if (eliteCount <= 0)
        {
            return offspring;
        }

        TChromosome[] elites = PopulationRanking.Best(problem.LossFunction, parents, eliteCount);
        TChromosome[] result = [.. offspring];
        int[] worst = PopulationRanking.WorstIndexes(problem.LossFunction, result, eliteCount);

        for (int i = 0; i < eliteCount; i++)
        {
            result[worst[i]] = elites[i];
        }

        return result;
    }

    private static IReadOnlyList<TChromosome> Resize(
        IReadOnlyList<TChromosome> population,
        int targetCount,
        IRandom random)
    {
        if (population.Count >= targetCount)
        {
            TChromosome[] truncated = new TChromosome[targetCount];
            for (int i = 0; i < targetCount; i++)
            {
                truncated[i] = population[i];
            }

            return truncated;
        }

        List<TChromosome> padded = new(targetCount);
        for (int i = 0; i < population.Count; i++)
        {
            padded.Add(population[i]);
        }

        if (population.Count == 0)
        {
            return padded;
        }

        while (padded.Count < targetCount)
        {
            padded.Add(population[random.Next(0, population.Count)].Clone());
        }

        return padded;
    }
}
