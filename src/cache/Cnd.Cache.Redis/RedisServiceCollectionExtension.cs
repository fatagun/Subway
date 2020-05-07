namespace Cnd.Cache.Redis
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class RedisServiceCollectionExtension
    {
        public static IServiceCollection AddRedisCache(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RedisOptions>(options => configuration.GetSection(typeof(RedisOptions).Name).Bind(options));

            services.Configure<RedisDbOptions>(options => configuration.GetSection(typeof(RedisDbOptions).Name).Bind(options));

            return services;
        }
    }
}
