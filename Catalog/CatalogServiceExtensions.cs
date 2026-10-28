using Microsoft.Extensions.DependencyInjection;

namespace Catalog;

/// <summary>Registration helpers for the catalog.</summary>
public static class CatalogServiceExtensions
{
    /// <summary>Registers the catalog as a singleton.</summary>
    /// <param name="services">The service collection to extend.</param>
    /// <returns>The same collection, for chaining.</returns>
    public static IServiceCollection AddCatalog(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<ICatalog, CatalogRegistry>();
        return services;
    }
}
