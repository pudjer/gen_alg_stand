using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Microsoft.JSInterop;
using Studio;

namespace Web.Storage;

/// <summary>
/// Persists experiment results in the browser's <c>localStorage</c> under a single key.
/// <para>
/// The domain records in <c>Studio</c> are not annotated for JSON and carry interface-typed
/// properties, so they are never serialized directly. A private DTO layer mirrors
/// <see cref="ExperimentResult"/> with concrete <see cref="Dictionary{TKey,TValue}"/> fields and
/// maps both ways. Serialization uses a source-generated context, so it is trim and AOT safe.
/// </para>
/// </summary>
public sealed class LocalStorageExperimentStore : IExperimentStore
{
    /// <summary>The <c>localStorage</c> key that holds every stored experiment.</summary>
    public const string StorageKey = "ga-bench.experiments";

    private static readonly JsonTypeInfo<List<StoredExperimentDto>> ListTypeInfo =
        StoredExperimentJsonContext.Default.ListStoredExperimentDto;

    private readonly IJSRuntime _js;
    private readonly SemaphoreSlim _gate = new(1, 1);

    /// <summary>Creates the store.</summary>
    /// <param name="js">The JS runtime used to reach <c>localStorage</c>.</param>
    public LocalStorageExperimentStore(IJSRuntime js)
    {
        ArgumentNullException.ThrowIfNull(js);
        _js = js;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<StoredExperiment>> ListAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            List<StoredExperimentDto> stored = await ReadAsync(cancellationToken).ConfigureAwait(false);

            return stored
                .OrderByDescending(item => item.SavedAtUtc)
                .Select(item => new Studio.StoredExperiment(item.Id, ToResult(item)))
                .ToArray();
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <inheritdoc />
    public async Task<string> SaveAsync(ExperimentResult result, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(result);

        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            List<StoredExperimentDto> stored = await ReadAsync(cancellationToken).ConfigureAwait(false);

            // Keep SavedAtUtc strictly increasing so "newest first" is deterministic even when
            // two saves land inside the same clock tick.
            DateTimeOffset now = DateTimeOffset.UtcNow;
            if (stored.Count > 0)
            {
                DateTimeOffset newest = stored.Max(item => item.SavedAtUtc);
                if (now <= newest)
                {
                    now = newest.AddTicks(1);
                }
            }

            string id = Guid.NewGuid().ToString("D");
            StoredExperimentDto dto = ToStored(result, now);
            dto.Id = id;

            stored.Add(dto);
            await WriteAsync(stored, cancellationToken).ConfigureAwait(false);
            return id;
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <inheritdoc />
    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);

        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            List<StoredExperimentDto> stored = await ReadAsync(cancellationToken).ConfigureAwait(false);

            int removed = stored.RemoveAll(item => string.Equals(item.Id, id, StringComparison.Ordinal));
            if (removed > 0)
            {
                await WriteAsync(stored, cancellationToken).ConfigureAwait(false);
            }
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task<List<StoredExperimentDto>> ReadAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string? json = await _js.InvokeAsync<string?>("localStorage.getItem", cancellationToken, StorageKey)
            .ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        try
        {
            List<StoredExperimentDto>? items = JsonSerializer.Deserialize(json, ListTypeInfo);
            if (items is null)
            {
                return [];
            }

            // A hand-edited payload may contain null entries; drop them instead of failing later.
            items.RemoveAll(static item => item is null);
            return items;
        }
        catch (JsonException)
        {
            // Corrupt or truncated payload: treat the store as empty rather than breaking the UI.
            return [];
        }
        catch (NotSupportedException)
        {
            return [];
        }
        catch (ArgumentException)
        {
            return [];
        }
    }

    private async Task WriteAsync(List<StoredExperimentDto> items, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string json = JsonSerializer.Serialize(items, ListTypeInfo);
        await _js.InvokeVoidAsync("localStorage.setItem", cancellationToken, StorageKey, json)
            .ConfigureAwait(false);
    }

    private static StoredExperimentDto ToStored(ExperimentResult result, DateTimeOffset savedAtUtc) => new()
    {
        Id = string.Empty,
        SavedAtUtc = savedAtUtc,
        Config = ToStored(result.Config),
        History = result.History.Select(ToStored).ToList(),
        BestLoss = result.BestLoss,
        TotalMilliseconds = result.TotalMilliseconds,
        Completed = result.Completed,
    };

    private static ExperimentResult ToResult(StoredExperimentDto dto) => new(
        ToConfig(dto.Config),
        (dto.History ?? []).Select(ToStats).ToArray(),
        dto.BestLoss,
        dto.TotalMilliseconds,
        dto.Completed);

    private static StoredConfig ToStored(ExperimentConfig config) => new()
    {
        ProblemKey = config.ProblemKey,
        ProblemParameters = Copy(config.ProblemParameters),
        AlgorithmKey = config.AlgorithmKey,
        AlgorithmParameters = Copy(config.AlgorithmParameters),
        CrossoverKey = config.CrossoverKey,
        CrossoverParameters = Copy(config.CrossoverParameters),
        MutationKey = config.MutationKey,
        MutationParameters = Copy(config.MutationParameters),
        SelectionKey = config.SelectionKey,
        SelectionParameters = Copy(config.SelectionParameters),
        InitialPopulationKey = config.InitialPopulationKey,
        InitialPopulationParameters = Copy(config.InitialPopulationParameters),
        Seed = config.Seed,
    };

    private static ExperimentConfig ToConfig(StoredConfig? config) => config is null
        ? new ExperimentConfig(
            string.Empty,
            new Dictionary<string, string>(StringComparer.Ordinal),
            string.Empty,
            new Dictionary<string, string>(StringComparer.Ordinal),
            string.Empty,
            new Dictionary<string, string>(StringComparer.Ordinal),
            string.Empty,
            new Dictionary<string, string>(StringComparer.Ordinal),
            string.Empty,
            new Dictionary<string, string>(StringComparer.Ordinal),
            string.Empty,
            new Dictionary<string, string>(StringComparer.Ordinal),
            0)
        : new ExperimentConfig(
            config.ProblemKey,
            config.ProblemParameters,
            config.AlgorithmKey,
            config.AlgorithmParameters,
            config.CrossoverKey,
            config.CrossoverParameters,
            config.MutationKey,
            config.MutationParameters,
            config.SelectionKey,
            config.SelectionParameters,
            config.InitialPopulationKey,
            config.InitialPopulationParameters,
            config.Seed);

    private static StoredGeneration ToStored(GenerationStats stats) => new()
    {
        Generation = stats.Generation,
        Best = stats.Best,
        Mean = stats.Mean,
        Worst = stats.Worst,
        StdDev = stats.StdDev,
        ElapsedMilliseconds = stats.ElapsedMilliseconds,
    };

    private static GenerationStats ToStats(StoredGeneration stats) => new(
        stats.Generation,
        stats.Best,
        stats.Mean,
        stats.Worst,
        stats.StdDev,
        stats.ElapsedMilliseconds);

    private static Dictionary<string, string> Copy(IReadOnlyDictionary<string, string> source)
    {
        Dictionary<string, string> copy = new(StringComparer.Ordinal);
        foreach (KeyValuePair<string, string> pair in source)
        {
            copy[pair.Key] = pair.Value;
        }

        return copy;
    }

    /// <summary>Storage envelope: a result plus the metadata the store needs to manage it.</summary>
    internal sealed class StoredExperimentDto
    {
        /// <summary>Gets or sets the stable identifier (a Guid formatted string).</summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>Gets or sets when the result was saved; list order is derived from this.</summary>
        public DateTimeOffset SavedAtUtc { get; set; }

        /// <summary>Gets or sets the configuration.</summary>
        public StoredConfig Config { get; set; } = new();

        /// <summary>Gets or sets the per-generation history.</summary>
        public List<StoredGeneration> History { get; set; } = [];

        /// <summary>Gets or sets the best loss.</summary>
        public double BestLoss { get; set; }

        /// <summary>Gets or sets the total duration.</summary>
        public double TotalMilliseconds { get; set; }

        /// <summary>Gets or sets whether the run completed.</summary>
        public bool Completed { get; set; }
    }

    /// <summary>Serializable mirror of <see cref="ExperimentConfig"/>.</summary>
    internal sealed class StoredConfig
    {
        /// <summary>Gets or sets the problem key.</summary>
        public string ProblemKey { get; set; } = string.Empty;

        /// <summary>Gets or sets the problem parameters.</summary>
        public Dictionary<string, string> ProblemParameters { get; set; } = new(StringComparer.Ordinal);

        /// <summary>Gets or sets the algorithm key.</summary>
        public string AlgorithmKey { get; set; } = string.Empty;

        /// <summary>Gets or sets the algorithm parameters.</summary>
        public Dictionary<string, string> AlgorithmParameters { get; set; } = new(StringComparer.Ordinal);

        /// <summary>Gets or sets the crossover key.</summary>
        public string CrossoverKey { get; set; } = string.Empty;

        /// <summary>Gets or sets the crossover parameters.</summary>
        public Dictionary<string, string> CrossoverParameters { get; set; } = new(StringComparer.Ordinal);

        /// <summary>Gets or sets the mutation key.</summary>
        public string MutationKey { get; set; } = string.Empty;

        /// <summary>Gets or sets the mutation parameters.</summary>
        public Dictionary<string, string> MutationParameters { get; set; } = new(StringComparer.Ordinal);

        /// <summary>Gets or sets the selection key.</summary>
        public string SelectionKey { get; set; } = string.Empty;

        /// <summary>Gets or sets the selection parameters.</summary>
        public Dictionary<string, string> SelectionParameters { get; set; } = new(StringComparer.Ordinal);

        /// <summary>Gets or sets the initial population key.</summary>
        public string InitialPopulationKey { get; set; } = string.Empty;

        /// <summary>Gets or sets the initial population parameters.</summary>
        public Dictionary<string, string> InitialPopulationParameters { get; set; } = new(StringComparer.Ordinal);

        /// <summary>Gets or sets the seed.</summary>
        public int Seed { get; set; }
    }

    /// <summary>Serializable mirror of <see cref="GenerationStats"/>.</summary>
    internal sealed class StoredGeneration
    {
        /// <summary>Gets or sets the generation index.</summary>
        public int Generation { get; set; }

        /// <summary>Gets or sets the best loss.</summary>
        public double Best { get; set; }

        /// <summary>Gets or sets the mean loss.</summary>
        public double Mean { get; set; }

        /// <summary>Gets or sets the worst loss.</summary>
        public double Worst { get; set; }

        /// <summary>Gets or sets the population standard deviation.</summary>
        public double StdDev { get; set; }

        /// <summary>Gets or sets the elapsed milliseconds.</summary>
        public double ElapsedMilliseconds { get; set; }
    }
}

/// <summary>Source-generated serialization context for the storage DTOs.</summary>
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true,
    WriteIndented = false)]
[JsonSerializable(typeof(LocalStorageExperimentStore.StoredExperimentDto))]
[JsonSerializable(typeof(List<LocalStorageExperimentStore.StoredExperimentDto>))]
internal sealed partial class StoredExperimentJsonContext : JsonSerializerContext;