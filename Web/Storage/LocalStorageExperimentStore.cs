using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Microsoft.JSInterop;
using Studio;

namespace Web.Storage;

public sealed class LocalStorageExperimentStore : IExperimentStore
{
    public const string StorageKey = "ga-bench.experiments";

    private static readonly JsonTypeInfo<List<StoredExperimentDto>> ListTypeInfo =
        StoredExperimentJsonContext.Default.ListStoredExperimentDto;

    private readonly IJSRuntime _js;
    private readonly SemaphoreSlim _gate = new(1, 1);

    public LocalStorageExperimentStore(IJSRuntime js)
    {
        ArgumentNullException.ThrowIfNull(js);
        _js = js;
    }

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

    public async Task<string> SaveAsync(ExperimentResult result, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(result);

        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            List<StoredExperimentDto> stored = await ReadAsync(cancellationToken).ConfigureAwait(false);

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

            items.RemoveAll(static item => item is null);
            return items;
        }
        catch (JsonException)
        {
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

    internal sealed class StoredExperimentDto
    {
        public string Id { get; set; } = string.Empty;

        public DateTimeOffset SavedAtUtc { get; set; }

        public StoredConfig Config { get; set; } = new();

        public List<StoredGeneration> History { get; set; } = [];

        public double BestLoss { get; set; }

        public double TotalMilliseconds { get; set; }

        public bool Completed { get; set; }
    }

    internal sealed class StoredConfig
    {
        public string ProblemKey { get; set; } = string.Empty;

        public Dictionary<string, string> ProblemParameters { get; set; } = new(StringComparer.Ordinal);

        public string AlgorithmKey { get; set; } = string.Empty;

        public Dictionary<string, string> AlgorithmParameters { get; set; } = new(StringComparer.Ordinal);

        public string CrossoverKey { get; set; } = string.Empty;

        public Dictionary<string, string> CrossoverParameters { get; set; } = new(StringComparer.Ordinal);

        public string MutationKey { get; set; } = string.Empty;

        public Dictionary<string, string> MutationParameters { get; set; } = new(StringComparer.Ordinal);

        public string SelectionKey { get; set; } = string.Empty;

        public Dictionary<string, string> SelectionParameters { get; set; } = new(StringComparer.Ordinal);

        public string InitialPopulationKey { get; set; } = string.Empty;

        public Dictionary<string, string> InitialPopulationParameters { get; set; } = new(StringComparer.Ordinal);

        public int Seed { get; set; }
    }

    internal sealed class StoredGeneration
    {
        public int Generation { get; set; }

        public double Best { get; set; }

        public double Mean { get; set; }

        public double Worst { get; set; }

        public double StdDev { get; set; }

        public double ElapsedMilliseconds { get; set; }
    }
}

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true,
    WriteIndented = false)]
[JsonSerializable(typeof(LocalStorageExperimentStore.StoredExperimentDto))]
[JsonSerializable(typeof(List<LocalStorageExperimentStore.StoredExperimentDto>))]
internal sealed partial class StoredExperimentJsonContext : JsonSerializerContext;