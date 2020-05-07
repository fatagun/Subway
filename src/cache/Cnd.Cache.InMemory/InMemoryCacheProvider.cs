namespace Cnd.Cache.InMemory
{
    using System;
    using System.Threading;
    using Cnd.Cache.Abstractions;
    using Cnd.Core.ServiceLifetime;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.Primitives;

    public sealed class InMemoryCacheProvider : IMemoryCacheProvider, ISingletonService
    {
        private static CancellationTokenSource _resetCacheToken = new CancellationTokenSource();
        private readonly MemoryCache _cache;
        private readonly IOptions<InMemoryCacheOptions> _options;
        private readonly ILogger<InMemoryCacheProvider> _logger;

        public InMemoryCacheProvider(
                                     IOptions<InMemoryCacheOptions> options,
                                     ILogger<InMemoryCacheProvider> logger
                                    )
        {


            _options = options;
            _logger = logger;


            _cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = options.Value.CacheSize,
                ExpirationScanFrequency = TimeSpan.FromSeconds(options.Value.ExpirationScanFrequency),
            });
        }

        private TimeSpan SlidingExpirationInSeconds => TimeSpan.FromSeconds(_options.Value.SlidingExpirationInSeconds);
        private TimeSpan AbsoluteExpirationInSeconds => TimeSpan.FromSeconds(_options.Value.AbsoluteExpirationInSeconds);

        private bool LoggingEnabled => _options.Value.EnableLogging;

        public void Set<T>(string key, T value)
            where T : class
        {
            Set(key, value, SlidingExpirationInSeconds, AbsoluteExpirationInSeconds);
        }

        public void Set<T>(string key, T value, TimeSpan slidingExpiration, TimeSpan absoluteExpiration)
            where T : class
        {

            if (LoggingEnabled)
            {
                _logger?.LogDebug($"Setting Cache = {key}");
            }
            _cache.Set(key, value, GetMemoryCacheEntryOptions(slidingExpiration, absoluteExpiration));
        }

        public T Get<T>(string key)
            where T : class
        {
            if (_cache.Get(key) is T result)
            {
                if (LoggingEnabled)
                {
                    _logger?.LogDebug($"cache hit = {key}");
                }

                return result;
            }

            _logger?.LogDebug($"cache miss = {key}");
            return default;
        }

        public T Get<T>(string key, Func<T> acquire)
            where T : class
        {
            if (_cache.TryGetValue(key, out T value))
            {
                if (value != null)
                {
                    if (LoggingEnabled)
                    {
                        _logger?.LogDebug($"cache hit = {key}");
                    }

                    return value;
                }
            }

            T result = acquire();

            if (LoggingEnabled)
            {
                _logger?.LogDebug($"Setting Cache = {key}");
            }

            _cache.Set(key, result, GetMemoryCacheEntryOptions(SlidingExpirationInSeconds, AbsoluteExpirationInSeconds));

            return result;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public void RemovePattern(string pattern)
        {
            throw new NotImplementedException();
        }

        public bool Exists(string key)
        {
            bool value = _cache.TryGetValue(key, out _);
            return value;
        }

        public void Refresh<T>(string key, T value)
            where T : class
        {
            Remove(key);
            Set(key, value);
        }

        public void Refresh<T>(string key, T value, TimeSpan expiration)
            where T : class
        {

            Remove(key);
            Set(key, value);
        }

        public void Flush()
        {
            if (LoggingEnabled)
            {
                _logger?.LogDebug("flushing cache");
            }

            if (_resetCacheToken != null && _resetCacheToken.IsCancellationRequested && _resetCacheToken.Token.CanBeCanceled)
            {
                _resetCacheToken.Cancel();
                _resetCacheToken.Dispose();
            }

            _resetCacheToken = new CancellationTokenSource();
        }

        private MemoryCacheEntryOptions GetMemoryCacheEntryOptions(TimeSpan slidingExpiration, TimeSpan absoluteExpiration)
        {
            var options = new MemoryCacheEntryOptions()
                .SetSize(1)
                .SetSlidingExpiration(slidingExpiration)
                .SetAbsoluteExpiration(absoluteExpiration)
                .AddExpirationToken(new CancellationChangeToken(_resetCacheToken.Token));

            return options;
        }

        public int Count() => _cache.Count;
    }
}
