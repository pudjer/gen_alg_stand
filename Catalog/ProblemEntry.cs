using System.Diagnostics;
using Algorithms.Observers;
using Catalog.Descriptors;
using Core.Contracts;
using Parameters;
using Studio;

namespace Catalog;

public sealed class ProblemOperators<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    public required ILossFunction<TChromosome> LossFunction { get; init; }

    public required IReadOnlyList<OperatorDescriptor> Crossovers { get; init; }

    public required IReadOnlyList<OperatorDescriptor> Mutations { get; init; }

    public required IReadOnlyList<OperatorDescriptor> Selections { get; init; }

    public required IReadOnlyList<OperatorDescriptor> InitialPopulations { get; init; }

    public required IReadOnlyList<OperatorDescriptor> Algorithms { get; init; }
}

public abstract class ProblemEntry<TChromosome> : IProblemEntry
    where TChromosome : IChromosome<TChromosome>
{
    private ProblemOperators<TChromosome>? _defaults;

    public abstract string Key { get; }

    public abstract string DisplayName { get; }

    public abstract IReadOnlyList<ParameterSpec> Parameters { get; }

    public IReadOnlyList<IOperatorDescriptor> Crossovers => Defaults.Crossovers;

    public IReadOnlyList<IOperatorDescriptor> Mutations => Defaults.Mutations;

    public IReadOnlyList<IOperatorDescriptor> Selections => Defaults.Selections;

    public IReadOnlyList<IOperatorDescriptor> InitialPopulations => Defaults.InitialPopulations;

    public IReadOnlyList<IOperatorDescriptor> Algorithms => Defaults.Algorithms;

    public IExperimentRunner CreateRunner(ExperimentConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        if (!string.Equals(config.ProblemKey, Key, StringComparison.Ordinal))
        {
            throw new ArgumentException(
                $"The configuration targets problem '{config.ProblemKey}' but this entry is '{Key}'.",
                nameof(config));
        }

        ParameterSet problemParameters = ParameterSet.Create(Parameters, config.ProblemParameters);
        ProblemOperators<TChromosome> operators = BuildOperators(problemParameters);

        ICrossover<TChromosome> crossover = Resolve<ICrossover<TChromosome>>(
            operators.Crossovers, config.CrossoverKey, config.CrossoverParameters);

        IMutation<TChromosome> mutation = Resolve<IMutation<TChromosome>>(
            operators.Mutations, config.MutationKey, config.MutationParameters);

        ISelection<TChromosome> selection = Resolve<ISelection<TChromosome>>(
            operators.Selections, config.SelectionKey, config.SelectionParameters);

        IInitialPopulation<TChromosome> initialPopulation = Resolve<IInitialPopulation<TChromosome>>(
            operators.InitialPopulations, config.InitialPopulationKey, config.InitialPopulationParameters);

        AlgorithmFactory algorithmFactory = Resolve<AlgorithmFactory>(
            operators.Algorithms, config.AlgorithmKey, config.AlgorithmParameters);

        IAlgorithm<TChromosome> algorithm = (IAlgorithm<TChromosome>)algorithmFactory.Create(config.Seed);

        return new ProblemRunner<TChromosome>(
            operators.LossFunction,
            crossover,
            mutation,
            selection,
            initialPopulation,
            algorithm);
    }

    protected abstract ProblemOperators<TChromosome> BuildOperators(ParameterSet problemParameters);

    private ProblemOperators<TChromosome> Defaults =>
        _defaults ??= BuildOperators(ParameterSet.FromDefaults(Parameters));

    private static TDescriptor Resolve<TDescriptor>(
        IReadOnlyList<OperatorDescriptor> descriptors,
        string key,
        IReadOnlyDictionary<string, string>? rawParameters)
        where TDescriptor : class
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        OperatorDescriptor? descriptor = null;
        for (int i = 0; i < descriptors.Count; i++)
        {
            if (string.Equals(descriptors[i].Key, key, StringComparison.Ordinal))
            {
                descriptor = descriptors[i];
                break;
            }
        }

        if (descriptor is null)
        {
            throw new ArgumentException($"Unknown operator '{key}'.", nameof(key));
        }

        ParameterSet parameters = ParameterSet.Create(descriptor.Parameters, rawParameters);
        object created = descriptor.Factory(parameters);

        return created as TDescriptor
            ?? throw new ArgumentException(
                $"Operator '{key}' did not produce a {typeof(TDescriptor).Name}.",
                nameof(key));
    }
}

internal sealed class ProblemRunner<TChromosome> : IExperimentRunner
    where TChromosome : IChromosome<TChromosome>
{
    private readonly ILossFunction<TChromosome> _lossFunction;
    private readonly ICrossover<TChromosome> _crossover;
    private readonly IMutation<TChromosome> _mutation;
    private readonly ISelection<TChromosome> _selection;
    private readonly IInitialPopulation<TChromosome> _initialPopulation;
    private readonly IAlgorithm<TChromosome> _algorithm;

    internal ProblemRunner(
        ILossFunction<TChromosome> lossFunction,
        ICrossover<TChromosome> crossover,
        IMutation<TChromosome> mutation,
        ISelection<TChromosome> selection,
        IInitialPopulation<TChromosome> initialPopulation,
        IAlgorithm<TChromosome> algorithm)
    {
        _lossFunction = lossFunction;
        _crossover = crossover;
        _mutation = mutation;
        _selection = selection;
        _initialPopulation = initialPopulation;
        _algorithm = algorithm;
    }

    public Task<ExperimentResult> RunAsync(
        ExperimentConfig config,
        IProgress<GenerationStats>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(config);

        return Task.Run(
            () => RunCore(config, progress, cancellationToken),
            CancellationToken.None);
    }

    private ExperimentResult RunCore(
        ExperimentConfig config,
        IProgress<GenerationStats>? progress,
        CancellationToken cancellationToken)
    {
        StatisticsObserver<TChromosome> observer = new(_lossFunction);
        List<GenerationStats> history = [];
        Stopwatch stopwatch = Stopwatch.StartNew();
        bool cancelled = false;

        object gate = new();
        observer.Updated += statistics =>
        {
            GenerationStats mapped = new(
                statistics.Generation,
                statistics.Best,
                statistics.Mean,
                statistics.Worst,
                statistics.StdDev,
                stopwatch.Elapsed.TotalMilliseconds);

            lock (gate)
            {
                history.Add(mapped);
            }

            progress?.Report(mapped);

            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException(cancellationToken);
            }
        };

        GeneticProblem<TChromosome> problem = new()
        {
            LossFunction = _lossFunction,
            Selection = _selection,
            Crossover = _crossover,
            Mutation = _mutation,
            InitialPopulation = _initialPopulation,
            Observer = observer,
        };

        try
        {
            _algorithm.Run(problem);
        }
        catch (OperationCanceledException)
        {
            cancelled = true;
        }

        stopwatch.Stop();

        GenerationStats[] snapshot;
        lock (gate)
        {
            snapshot = [.. history];
        }

        double bestLoss = double.NaN;
        for (int i = 0; i < snapshot.Length; i++)
        {
            if (i == 0 || snapshot[i].Best < bestLoss)
            {
                bestLoss = snapshot[i].Best;
            }
        }

        return new ExperimentResult(
            config,
            snapshot,
            bestLoss,
            stopwatch.Elapsed.TotalMilliseconds,
            !cancelled && !cancellationToken.IsCancellationRequested);
    }
}
