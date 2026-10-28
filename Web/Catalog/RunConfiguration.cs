using System.Diagnostics;
using Algorithms.Orchestration;
using Core.Contracts;
using Parameters;
using Setup;

namespace GenAlgStand.Web.Catalog;

public sealed class RunConfiguration<TChromosome> : IRunConfiguration
    where TChromosome : IChromosome<TChromosome>
{
    private readonly IReadOnlyList<Variant<TChromosome>> _variants;
    private readonly Func<TChromosome, string> _describe;
    private readonly IProblemVisualizer<TChromosome>? _visualizer;

    public RunConfiguration(
        IReadOnlyList<Variant<TChromosome>> variants,
        Func<TChromosome, string> describe,
        IProblemVisualizer<TChromosome>? visualizer = null)
    {
        _variants = variants;
        _describe = describe;
        _visualizer = visualizer;

        LossFunctions = Options(variants.Select(variant => variant.LossFunction));
        Selections = Options(variants.Select(variant => variant.Selection));
        Crossovers = Options(variants.Select(variant => variant.Crossover));
        Mutations = Options(variants.Select(variant => variant.Mutation));
        InitialPopulations = Options(variants.Select(variant => variant.InitialPopulation));
    }

    public IReadOnlyList<ModuleOption> LossFunctions { get; }

    public IReadOnlyList<ModuleOption> Selections { get; }

    public IReadOnlyList<ModuleOption> Crossovers { get; }

    public IReadOnlyList<ModuleOption> Mutations { get; }

    public IReadOnlyList<ModuleOption> InitialPopulations { get; }

    public RunResult Run(RunRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        ModuleOption loss = Pick(LossFunctions, request.Loss, "функция потерь");
        ModuleOption selection = Pick(Selections, request.Selection, "селекция");
        ModuleOption crossover = Pick(Crossovers, request.Crossover, "скрещивание");
        ModuleOption mutation = Pick(Mutations, request.Mutation, "мутация");
        ModuleOption initial = Pick(InitialPopulations, request.InitialPopulation, "начальная популяция");

        Variant<TChromosome>? variant = _variants.FirstOrDefault(candidate =>
            candidate.LossFunction.Name == loss.Name &&
            candidate.Selection.Name == selection.Name &&
            candidate.Crossover.Name == crossover.Name &&
            candidate.Mutation.Name == mutation.Name &&
            candidate.InitialPopulation.Name == initial.Name)
            ?? throw new RunRejectedException("Выбранная комбинация модулей недоступна для этой задачи.");

        GeneticProblem<TChromosome> problem = variant.Create(
            BuildSet("loss", loss, request),
            BuildSet("selection", selection, request),
            BuildSet("crossover", crossover, request),
            BuildSet("mutation", mutation, request),
            BuildSet("init", initial, request));

        var observer = new SubscribableObserver<TChromosome>();
        var runtime = new Runtime<TChromosome>
        {
            Observer = observer,
            RandomFactory = new SeededRandomFactory(request.Seed),
        };

        int maxPoints = Math.Max(2, request.MaxPoints);
        int stride = Math.Max(1, (int)Math.Ceiling((request.Generations + 1.0) / (maxPoints - 1)));
        var samples = new List<Sample>(maxPoints);
        int generation = 0;
        double bestEver = double.PositiveInfinity;
        TChromosome? champion = default;
        bool stopped = false;
        double lastAverage = 0;

        var stopwatch = Stopwatch.StartNew();

        using IDisposable subscription = observer.Subscribe(population =>
        {
            if (population.Count == 0)
            {
                generation++;
                return;
            }

            if (generation == 0)
            {
                if (population.Count > 100_000)
                {
                    throw new RunRejectedException(
                        $"Популяция из {population.Count:N0} особей слишком велика для стенда (лимит 100 000).");
                }

                long evaluations = (long)population.Count * (request.Generations + 1);
                if (evaluations > request.MaxEvaluations)
                {
                    throw new RunRejectedException(
                        $"Слишком большой объём: {population.Count} особей × {request.Generations + 1} поколений " +
                        $"= {evaluations:N0} оценок, а лимит — {request.MaxEvaluations:N0}. " +
                        "Уменьшите размер популяции или число поколений.");
                }
            }

            double sum = 0;
            double best = double.PositiveInfinity;
            TChromosome? generationChampion = default;
            double generationBest = double.PositiveInfinity;

            foreach (Individ<TChromosome> individual in population)
            {
                double value = individual.LossValue;
                sum += value;

                if (value < best)
                {
                    best = value;
                }

                if (value < generationBest)
                {
                    generationBest = value;
                    generationChampion = individual.Chromosome;
                }
            }

            if (generationBest < bestEver)
            {
                bestEver = generationBest;
                champion = generationChampion;
            }

            lastAverage = sum / population.Count;

            if (generation % stride == 0)
            {
                samples.Add(new Sample(generation, lastAverage, best));
            }

            generation++;

            if (stopwatch.Elapsed > request.Budget)
            {
                stopped = true;
                throw new RunBudgetExceededException();
            }
        });

        try
        {
            var algorithm = new GenerationalAlgorithm<TChromosome>(
                new GenerationalOptions(request.Generations, request.EliteCount, MaintainPopulationSize: true));

            algorithm.Run(problem, runtime);
        }
        catch (RunBudgetExceededException)
        {
            // Результаты, собранные до остановки, остаются валидными.
        }

        stopwatch.Stop();

        if (samples.Count == 0)
        {
            samples.Add(new Sample(0, lastAverage, double.IsPositiveInfinity(bestEver) ? lastAverage : bestEver));
        }

        int lastGeneration = Math.Max(0, generation - 1);
        if (samples[^1].Generation < lastGeneration)
        {
            samples.Add(new Sample(lastGeneration, lastAverage, bestEver));
        }

        Visualization? visualization = null;
        if (_visualizer is not null && champion is not null)
        {
            try
            {
                visualization = _visualizer.Build(
                    new VisualizationContext<TChromosome>(champion, problem.LossFunction, samples));
            }
            catch
            {
                // Опциональная визуализация не должна ронять уже завершённый прогон.
                visualization = null;
            }
        }

        return new RunResult(
            samples,
            bestEver,
            champion is null ? string.Empty : _describe(champion),
            lastGeneration,
            stopwatch.Elapsed.TotalSeconds,
            stopped,
            $"{loss.Name} / {selection.Name} / {crossover.Name} / {mutation.Name} / {initial.Name}",
            visualization);
    }

    private static ModuleOption Pick(IReadOnlyList<ModuleOption> options, int index, string axis)
    {
        if (index < 0 || index >= options.Count)
        {
            throw new RunRejectedException($"Модуль «{axis}» выбран некорректно. Обновите список модулей.");
        }

        return options[index];
    }

    private static ParameterSet BuildSet(string axis, ModuleOption option, RunRequest request)
    {
        var values = new ParameterSet();

        foreach (IParameter parameter in option.Parameters)
        {
            string key = $"{axis}_{option.Index}_{parameter.Key}";
            if (request.ModuleParameters.TryGetValue(key, out string? raw) && !string.IsNullOrWhiteSpace(raw))
            {
                values[parameter.Key] = raw.Trim();
            }
        }

        return values;
    }

    private static IReadOnlyList<ModuleOption> Options<TModule>(IEnumerable<IModuleFactory<TModule>> factories) =>
        factories
            .DistinctBy(factory => factory.Name)
            .Select((factory, index) => new ModuleOption(index, factory.Name, factory.Parameters))
            .ToArray();
}