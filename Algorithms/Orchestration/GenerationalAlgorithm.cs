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

    public void Run(GeneticProblem<TChromosome> problem, IRuntime<TChromosome> runtime)
    {
        IRandom random = runtime.RandomFactory.Create()
            ?? throw new InvalidOperationException("The random factory must not return null.");

        IReadOnlyList<TChromosome> initial = problem.InitialPopulation.Create(random)
            ?? throw new InvalidOperationException("The initial population must not be null.");

        IReadOnlyList<Individ<TChromosome>> population = Wrap(initial, problem.LossFunction);

        runtime.Observer.OnGeneration(population);

        for (int generation = 0; generation < _options.MaxGenerations; generation++)
        {
            IReadOnlyList<Individ<TChromosome>>? next = Step(problem, population, random);
            if (next is null)
            {
                return;
            }

            population = next;
            runtime.Observer.OnGeneration(population);
        }
    }

    internal IReadOnlyList<Individ<TChromosome>>? Step(
        GeneticProblem<TChromosome> problem,
        IReadOnlyList<Individ<TChromosome>> population,
        IRandom random)
    {
        if (population.Count == 0)
        {
            return null;
        }

        IReadOnlyList<Individ<TChromosome>> selected = problem.Selection.Select(population, random);
        IReadOnlyList<TChromosome> children = problem.Crossover.Crossover(selected, random);
        IReadOnlyList<TChromosome> mutated = problem.Mutation.Mutate(Wrap(children, problem.LossFunction), random);
        IReadOnlyList<Individ<TChromosome>> offspring = Wrap(mutated, problem.LossFunction);
        IReadOnlyList<Individ<TChromosome>> next = ApplyElitism(population, offspring);

        if (_options.MaintainPopulationSize && next.Count != population.Count)
        {
            next = Resize(next, population.Count, random);
        }

        return next;
    }

    internal static IReadOnlyList<Individ<TChromosome>> Wrap(
        IReadOnlyList<TChromosome> chromosomes,
        ILossFunction<TChromosome> lossFunction)
    {
        int count = chromosomes.Count;
        if (count == 0)
        {
            return Array.Empty<Individ<TChromosome>>();
        }

        Individ<TChromosome>[] individs = new Individ<TChromosome>[count];
        for (int i = 0; i < count; i++)
        {
            individs[i] = new Individ<TChromosome>(chromosomes[i], lossFunction);
        }

        return individs;
    }

    private IReadOnlyList<Individ<TChromosome>> ApplyElitism(
        IReadOnlyList<Individ<TChromosome>> parents,
        IReadOnlyList<Individ<TChromosome>> offspring)
    {
        int eliteCount = Math.Min(_options.EliteCount, Math.Min(parents.Count, offspring.Count));
        if (eliteCount <= 0)
        {
            return offspring;
        }

        Individ<TChromosome>[] elites = PopulationRanking.Best(parents, eliteCount);
        Individ<TChromosome>[] result = [.. offspring];
        int[] worst = PopulationRanking.WorstIndexes(result, eliteCount);

        for (int i = 0; i < eliteCount; i++)
        {
            result[worst[i]] = elites[i];
        }

        return result;
    }

    private static IReadOnlyList<Individ<TChromosome>> Resize(
        IReadOnlyList<Individ<TChromosome>> population,
        int targetCount,
        IRandom random)
    {
        if (population.Count >= targetCount)
        {
            Individ<TChromosome>[] truncated = new Individ<TChromosome>[targetCount];
            for (int i = 0; i < targetCount; i++)
            {
                truncated[i] = population[i];
            }

            return truncated;
        }

        List<Individ<TChromosome>> padded = new(targetCount);
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
            padded.Add(population[random.Next(0, population.Count)]);
        }

        return padded;
    }
}
