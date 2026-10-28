using GenAlgStand.Web.Catalog;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = ResolveContentRoot(),
});

builder.Services.AddRazorPages();
builder.Services.AddSingleton<ProblemCatalog>();

var app = builder.Build();

app.UseStaticFiles();
app.MapRazorPages();

app.Run();

static string ResolveContentRoot()
{
    string current = Directory.GetCurrentDirectory();
    if (Directory.Exists(Path.Combine(current, "wwwroot")))
    {
        return current;
    }

    string? directory = AppContext.BaseDirectory;
    while (!string.IsNullOrEmpty(directory))
    {
        if (Directory.Exists(Path.Combine(directory, "wwwroot")))
        {
            return directory;
        }

        directory = Path.GetDirectoryName(directory);
    }

    return current;
}