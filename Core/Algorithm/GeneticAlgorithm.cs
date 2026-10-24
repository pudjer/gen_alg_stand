using System.Diagnostics;
using Core.Contracts.Inner;
using Core.Contracts.Outer;

namespace Core.Algorithm;

/// <summary>
/// Главный цикл генетического алгоритма.
/// Работает исключительно через абстракции: не знает ни о кодировании,
/// ни о целевой функции, ни о конкретных операторах.
/// Размер популяции — свойство задачи, а не настройка движка.
/// </summary>
public sealed class GeneticAlgorithm : IGeneticAlgorithm
{
    public IGeneticAlgorithmResult Run(
        IGeneticProblem problem,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(problem);

        var random = new RandomSource();
        var stopwatch = Stopwatch.StartNew();
        var maximize = problem.FitnessFunction.IsMaximization;
        var context = new ExecutionContext(problem.FitnessFunction, random, cancellationToken);

        var population = CreateInitialPopulation(problem, context);
        var best = FindBest(population, maximize);
        var generation = 0;

        while (true)
        {
            var generationBest = FindBest(population, maximize);

            if (IsBetter(generationBest.Fitness, best.Fitness, maximize))
            {
                best = generationBest;
            }

            context.Record(new GenerationResult(
                generation,
                generationBest.Fitness,
                population.Average(individual => individual.Fitness)));

            context.Advance(generation, population, best);

            if (problem.Termination.ShouldStop(context))
            {
                break;
            }

            population = CreateNextGeneration(problem, context);
            generation++;
        }

        stopwatch.Stop();

        return new GeneticAlgorithmResult(best, context.History, stopwatch.Elapsed);
    }

    private static IReadOnlyList<Individual> CreateInitialPopulation(
        IGeneticProblem problem,
        ExecutionContext context)
    {
        var chromosomes = problem.InitialPopulation.Create(context);
        var population = new List<Individual>(chromosomes.Count);

        foreach (var chromosome in chromosomes)
        {
            population.Add(Evaluate(problem, chromosome, context));
        }

        return population;
    }

    private static IReadOnlyList<Individual> CreateNextGeneration(
        IGeneticProblem problem,
        ExecutionContext context)
    {
        var parents = problem.Selection.Select(context);
        var next = new List<Individual>(parents.Count);

        for (var i = 0; i + 1 < parents.Count; i += 2)
        {
            var (first, second) = problem.Crossover.Crossover(
                parents[i].Chromosome,
                parents[i + 1].Chromosome,
                context);

            next.Add(Evaluate(problem, problem.Mutation.Mutate(first, context), context));
            next.Add(Evaluate(problem, problem.Mutation.Mutate(second, context), context));
        }

        // Нечётное число родителей: последний продолжает род через мутацию.
        if (parents.Count % 2 == 1)
        {
            var last = parents[^1].Chromosome.Clone();
            next.Add(Evaluate(problem, problem.Mutation.Mutate(last, context), context));
        }

        return next;
    }

    private static Individual Evaluate(
        IGeneticProblem problem,
        IChromosome chromosome,
        ExecutionContext context)
        => new(chromosome, problem.FitnessFunction.Evaluate(chromosome, context));

    private static Individual FindBest(IReadOnlyList<Individual> population, bool maximize)
    {
        var best = population[0];

        for (var i = 1; i < population.Count; i++)
        {
            if (IsBetter(population[i].Fitness, best.Fitness, maximize))
            {
                best = population[i];
            }
        }

        return best;
    }

    private static bool IsBetter(double candidate, double incumbent, bool maximize)
        => maximize ? candidate > incumbent : candidate < incumbent;
}
