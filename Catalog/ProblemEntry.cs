using System.Diagnostics;
using Algorithms.Observers;
using Catalog.Descriptors;
using Core.Contracts;
using Parameters;
using Studio;

namespace Catalog;

/// <summary>
/// Everything a problem needs to run, already closed over its chromosome type: the loss function
/// plus the operator descriptors that are valid for the current problem parameters.
/// </summary>
/// <typeparam name="TChromosome">The chromosome type of the problem.</typeparam>
public sealed class ProblemOperators<TChromosome>
    where TChromosome : IChromosome<TChromosome>
{
    /// <summary>Gets the loss function of the problem.</summary>
    public required ILossFunction<TChromosome> LossFunction { get; init; }

    /// <summary>Gets the valid crossovers.</summary>
    public required IReadOnlyList<OperatorDescriptor> Crossovers { get; init; }

    /// <summary>Gets the valid mutations.</summary>
    public required IReadOnlyList<OperatorDescriptor> Mutations { get; init; }

    /// <summary>Gets the valid selections.</summary>
    public required IReadOnlyList<OperatorDescriptor> Selections { get; init; }

    /// <summary>Gets the valid initial population generators.</summary>
    public required IReadOnlyList<OperatorDescriptor> InitialPopulations { get; init; }

    /// <summary>Gets the valid algorithms.</summary>
    public required IReadOnlyList<OperatorDescriptor> Algorithms { get; init; }
}

/// <summary>
/// The generic half of a problem entry. It exposes the non-generic <see cref="IProblemEntry"/> view
/// and resolves the chromosome type parameter in exactly one place: <see cref="CreateRunner"/>.
/// </summary>
/// <typeparam name="TChromosome">The chromosome type of the problem.</typeparam>
public abstract class ProblemEntry<TChromosome> : IProblemEntry
    where TChromosome : IChromosome<TChromosome>
{
    private ProblemOperators<TChromosome>? _defaults;

    /// <inheritdoc />
    public abstract string Key { get; }

    /// <inheritdoc />
    public abstract string DisplayName { get; }

    /// <inheritdoc />
    public abstract IReadOnlyList<ParameterSpec> Parameters { get; }

    /// <inheritdoc />
    public IReadOnlyList<IOperatorDescriptor> Crossovers => Defaults.Crossovers;

    /// <inheritdoc />
    public IReadOnlyList<IOperatorDescriptor> Mutations => Defaults.Mutations;

    /// <inheritdoc />
    public IReadOnlyList<IOperatorDescriptor> Selections => Defaults.Selections;

    /// <inheritdoc />
    public IReadOnlyList<IOperatorDescriptor> InitialPopulations => Defaults.InitialPopulations;

    /// <inheritdoc />
    public IReadOnlyList<IOperatorDescriptor> Algorithms => Defaults.Algorithms;

    /// <inheritdoc />
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

    /// <summary>Builds the operator set for a concrete problem-parameter set.</summary>
    /// <param name="problemParameters">Validated problem parameters.</param>
    /// <returns>The operators valid for those parameters.</returns>
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

/// <summary>
/// Runs one configured experiment for a concrete chromosome type, collecting per-generation
/// statistics and honouring cancellation.
/// </summary>
/// <typeparam name="TChromosome">The chromosome type of the problem.</typeparam>
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

    /// <inheritdoc />
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

        // The observer runs on the algorithm's thread. Progress is reported through IProgress<T>,
        // which marshals to the captured synchronization context on its own, so no UI state is
        // touched here. The history list is guarded because the island model may call back from
        // several threads in principle.
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
