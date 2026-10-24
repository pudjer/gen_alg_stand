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
/// <typeparam name="TChromosome">Конкретный тип хромосомы задачи.</typeparam>
public sealed class GeneticAlgorithm<TChromosome> : IGeneticAlgorithm<TChromosome>
    where TChromosome : IChromosome
{
    public IGeneticAlgorithmResult<TChromosome> Run(
        IGeneticProblem<TChromosome> problem,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(problem);
        ValidateComponents(problem);

        var maximize = problem.FitnessFunction.IsMaximization;
        var random = new RandomSource();
        var stopwatch = Stopwatch.StartNew();
        var context = new ExecutionContext<TChromosome>(problem, random, cancellationToken);

        var population = CreateInitialPopulation(context);
        var generation = 0;

        // Лучшая хромосома ищется ровно раз на поколение. Поиск нулевого
        // поколения засевает рекорд прогона: второго прохода по той же
        // популяции нет.
        var bestIndex = FindBestIndex(population, maximize);
        var best = population[bestIndex];
        var bestFitness = population.GetFitness(bestIndex);
        var generationBest = bestFitness;

        while (true)
        {
            context.Record(new GenerationResult(
                generation,
                generationBest,
                Average(population)));

            context.Advance(generation, population, best, bestFitness);

            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            if (problem.Termination.ShouldStop(context))
            {
                break;
            }

            population = CreateNextGeneration(context);
            generation++;

            var index = FindBestIndex(population, maximize);
            generationBest = population.GetFitness(index);

            if (IsBetter(generationBest, bestFitness, maximize))
            {
                best = population[index];
                bestFitness = generationBest;
            }
        }

        stopwatch.Stop();

        return new GeneticAlgorithmResult<TChromosome>(
            best,
            bestFitness,
            context.History,
            stopwatch.Elapsed);
    }

    private static IPopulation<TChromosome> CreateInitialPopulation(
        ExecutionContext<TChromosome> context)
    {
        var chromosomes = RequirePopulation(
            context.Problem.InitialPopulation.Create(context.Random),
            nameof(IInitialPopulation<TChromosome>.Create));

        return Build(context, chromosomes);
    }

    private static IPopulation<TChromosome> CreateNextGeneration(
        ExecutionContext<TChromosome> context)
    {
        var problem = context.Problem;

        var parents = RequirePopulation(
            problem.Selection.Select(
                context.Population,
                problem.FitnessFunction.IsMaximization,
                context.Random),
            nameof(ISelection<TChromosome>.Select));

        var next = new List<TChromosome>(parents.Count);

        for (var i = 0; i + 1 < parents.Count; i += 2)
        {
            var (first, second) = MaybeCrossover(context, parents[i], parents[i + 1]);

            next.Add(MaybeMutate(context, first));
            next.Add(MaybeMutate(context, second));
        }

        // Нечётное число родителей: последний продолжает род через мутацию.
        if (parents.Count % 2 == 1)
        {
            next.Add(MaybeMutate(context, parents[^1]));
        }

        return Build(context, next);
    }

    /// <summary>
    /// Скрещивает пару с вероятностью <c>CrossoverProbability</c>;
    /// в противном случае возвращает родителей без изменений.
    /// </summary>
    private static (TChromosome First, TChromosome Second) MaybeCrossover(
        ExecutionContext<TChromosome> context,
        TChromosome first,
        TChromosome second)
    {
        if (context.Random.NextDouble() < context.Problem.CrossoverProbability)
        {
            return context.Problem.Crossover.Crossover(first, second, context.Random);
        }

        return (first, second);
    }

    /// <summary>
    /// Мутирует хромосому с вероятностью <c>MutationProbability</c>;
    /// в противном случае возвращает её без изменений.
    /// </summary>
    private static TChromosome MaybeMutate(
        ExecutionContext<TChromosome> context,
        TChromosome chromosome)
        => context.Random.NextDouble() < context.Problem.MutationProbability
            ? context.Problem.Mutation.Mutate(chromosome, context.Random)
            : chromosome;

    /// <summary>Оценивает хромосомы и упаковывает их вместе с оценками.</summary>
    private static IPopulation<TChromosome> Build(
        ExecutionContext<TChromosome> context,
        IReadOnlyList<TChromosome> chromosomes)
    {
        var fitnessFunction = context.Problem.FitnessFunction;
        var fitness = new double[chromosomes.Count];

        for (var i = 0; i < chromosomes.Count; i++)
        {
            fitness[i] = fitnessFunction.Evaluate(chromosomes[i]);
        }

        return new Population<TChromosome>(chromosomes, fitness);
    }

    /// <summary>
    /// Проверяет, что компонент задачи вернул пригодный список хромосом.
    /// Пустой список ломает инвариант движка: выбирать лучшую не из чего.
    /// </summary>
    private static IReadOnlyList<TChromosome> RequirePopulation(
        IReadOnlyList<TChromosome>? chromosomes,
        string source)
    {
        if (chromosomes is null)
        {
            throw new InvalidOperationException(
                $"{source} вернул null вместо списка хромосом.");
        }

        if (chromosomes.Count == 0)
        {
            throw new InvalidOperationException(
                $"{source} вернул пустой список: популяция не может быть пустой.");
        }

        return chromosomes;
    }

    /// <summary>
    /// Отсутствие компонента — ошибка сборки задачи, а не рантайма:
    /// ловим её до старта, а не в середине цикла.
    /// </summary>
    private static void ValidateComponents(IGeneticProblem<TChromosome> problem)
    {
        ArgumentNullException.ThrowIfNull(problem.FitnessFunction);
        ArgumentNullException.ThrowIfNull(problem.InitialPopulation);
        ArgumentNullException.ThrowIfNull(problem.Selection);
        ArgumentNullException.ThrowIfNull(problem.Crossover);
        ArgumentNullException.ThrowIfNull(problem.Mutation);
        ArgumentNullException.ThrowIfNull(problem.Termination);

        ValidateProbability(problem.CrossoverProbability, nameof(problem.CrossoverProbability));
        ValidateProbability(problem.MutationProbability, nameof(problem.MutationProbability));
    }

    /// <summary>
    /// Вероятность вне отрезка <c>[0, 1]</c> — ошибка сборки задачи,
    /// а не рантайма: ловим её до старта, а не в середине цикла.
    /// </summary>
    private static void ValidateProbability(double probability, string source)
    {
        if (probability is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(
                source,
                probability,
                "Вероятность должна лежать в отрезке [0, 1].");
        }
    }

    /// <summary>
    /// Индекс лучшей хромосомы поколения. Индекс берётся один раз и затем
    /// читается в паре <c>population[index]</c> / <c>GetFitness(index)</c>,
    /// поэтому хромосома и её оценка разойтись не могут.
    /// </summary>
    private static int FindBestIndex(IPopulation<TChromosome> population, bool maximize)
    {
        var bestIndex = 0;

        for (var i = 1; i < population.Count; i++)
        {
            if (IsBetter(population.GetFitness(i), population.GetFitness(bestIndex), maximize))
            {
                bestIndex = i;
            }
        }

        return bestIndex;
    }

    private static double Average(IPopulation<TChromosome> population)
    {
        var sum = 0.0;

        for (var i = 0; i < population.Count; i++)
        {
            sum += population.GetFitness(i);
        }

        return sum / population.Count;
    }

    private static bool IsBetter(double candidate, double incumbent, bool maximize)
        => maximize ? candidate > incumbent : candidate < incumbent;
}