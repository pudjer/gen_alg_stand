using Microsoft.Extensions.DependencyInjection;

namespace Catalog;

public static class CatalogServiceExtensions
{
    public static IServiceCollection AddCatalog(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<ICatalog, CatalogRegistry>();
        return services;
    }
}
