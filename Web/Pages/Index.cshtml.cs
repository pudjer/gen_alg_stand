using System.Text.Json;
using GenAlgStand.Web.Catalog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Parameters;

namespace GenAlgStand.Web.Pages;

public sealed class IndexModel : PageModel
{
    public const int MaxEvaluations = 2_000_000;
    public const int MaxPoints = 1500;

    private static readonly TimeSpan Budget = TimeSpan.FromSeconds(20);

    private readonly ProblemCatalog _catalog;

    public IndexModel(ProblemCatalog catalog) => _catalog = catalog;

    [BindProperty]
    public string? ProblemId { get; set; }

    [BindProperty]
    public string? ConfiguredProblemId { get; set; }

    [BindProperty]
    public Dictionary<string, string?> ProblemParams { get; set; } = new();

    [BindProperty]
    public Dictionary<string, string?> ModuleParams { get; set; } = new();

    [BindProperty]
    public Dictionary<string, string?> ModuleChoice { get; set; } = new();

    [BindProperty]
    public int Generations { get; set; } = 1000;

    [BindProperty]
    public int EliteCount { get; set; } = 1;

    [BindProperty]
    public int Seed { get; set; } = 42;

    public IReadOnlyList<IProblemDescriptor> Problems => _catalog.All;

    public IProblemDescriptor? Descriptor { get; private set; }

    public IRunConfiguration? Configuration { get; private set; }

    public IReadOnlyList<AxisView> Axes { get; private set; } = [];

    public RunResult? Result { get; private set; }

    public string? Error { get; private set; }

    public string? ChartDataJson { get; private set; }

    public void OnGet(string? problem)
    {
        ProblemId = _catalog.Find(problem) is not null ? problem : "sphere";
        TryConfigure();
    }

    public IActionResult OnPostConfigure()
    {
        if (TryConfigure())
        {
            Error = null;
        }

        return Page();
    }

    public IActionResult OnPostRun()
    {
        string? configuredBefore = ConfiguredProblemId;

        if (!TryConfigure())
        {
            return Page();
        }

        if (!string.Equals(ProblemId, configuredBefore, StringComparison.Ordinal))
        {
            Error = "Задача изменилась. Проверьте параметры и модули, затем запустите ещё раз.";
            return Page();
        }

        if (Generations is < 1 or > 500_000)
        {
            Error = "Число поколений должно быть от 1 до 500 000.";
            return Page();
        }

        if (EliteCount is < 0 or > 10_000)
        {
            Error = "Число элитных особей должно быть от 0 до 10 000.";
            return Page();
        }

        var request = new RunRequest(
            Axes[0].Selected,
            Axes[1].Selected,
            Axes[2].Selected,
            Axes[3].Selected,
            Axes[4].Selected,
            Axes[5].Selected,
            ModuleParams,
            Generations,
            EliteCount,
            Seed,
            MaxPoints,
            MaxEvaluations,
            Budget);

        try
        {
            Result = Configuration!.Run(request);
        }
        catch (RunRejectedException exception)
        {
            Error = exception.Message;
            return Page();
        }
        catch (Exception exception)
        {
            Error = exception.Message;
            return Page();
        }

        ChartDataJson = JsonSerializer.Serialize(
            Result.Samples.Select(sample => new[]
            {
                sample.Generation,
                Sanitize(sample.Average),
                Sanitize(sample.Best),
            }));

        return Page();
    }

    public string ProblemValue(string key) => Lookup(ProblemParams, key);

    public string ModuleValue(string name) => Lookup(ModuleParams, name);

    private bool TryConfigure()
    {
        Descriptor = _catalog.Find(ProblemId);
        if (Descriptor is null)
        {
            Error = $"Неизвестная задача: «{ProblemId}».";
            Configuration = null;
            Axes = [];
            return false;
        }

        var values = new ParameterSet();
        foreach (IParameter parameter in Descriptor.Parameters)
        {
            string raw = ProblemValue(parameter.Key);
            if (!string.IsNullOrWhiteSpace(raw))
            {
                values[parameter.Key] = raw.Trim();
            }
        }

        try
        {
            Configuration = Descriptor.Configure(values);
        }
        catch (Exception exception)
        {
            Error = exception.Message;
            Configuration = null;
            Axes = [];
            return false;
        }

        Axes =
        [
            Axis("algorithm", "Алгоритм", Configuration.Algorithms),
            Axis("loss", "Функция потерь", Configuration.LossFunctions),
            Axis("selection", "Селекция", Configuration.Selections),
            Axis("crossover", "Скрещивание", Configuration.Crossovers),
            Axis("mutation", "Мутация", Configuration.Mutations),
            Axis("init", "Начальная популяция", Configuration.InitialPopulations),
        ];

        ConfiguredProblemId = ProblemId;
        return true;
    }

    private AxisView Axis(string key, string title, IReadOnlyList<ModuleOption> options)
    {
        int selected = 0;
        if (ModuleChoice.TryGetValue(key, out string? raw) &&
            int.TryParse(raw, out int parsed) &&
            parsed >= 0 &&
            parsed < options.Count)
        {
            selected = parsed;
        }

        return new AxisView(key, title, options, selected);
    }

    private static string Lookup(IReadOnlyDictionary<string, string?> map, string key) =>
        map.TryGetValue(key, out string? value) ? value ?? string.Empty : string.Empty;

    private static double Sanitize(double value) => double.IsFinite(value) ? value : 0;

    public sealed record AxisView(
        string Key,
        string Title,
        IReadOnlyList<ModuleOption> Options,
        int Selected);
}