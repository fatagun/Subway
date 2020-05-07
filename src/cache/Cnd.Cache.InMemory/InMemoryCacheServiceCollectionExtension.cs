namespace Cnadf.Cache.InMemory
{
    using Cnd.Cache.Abstractions;
    using Cnd.Cache.InMemory;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class InMemoryCacheServiceCollectionExtension
    {
        public static IServiceCollection AddInMemoryCache(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<InMemoryCacheOptions>(options => configuration.GetSection("InMemoryCache").Bind(options));

            services.AddSingleton<OldInMemoryCacheProvider>();

            services.AddMemoryCache();

            return services;
        }
    }
}
