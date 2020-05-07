namespace Cnd.Cache.InMemory
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Primitives;

    public sealed class OldInMemoryCacheProvider
    {
        private static CancellationTokenSource _resetCacheToken = new CancellationTokenSource();
        private readonly IMemoryCache _cache;
        private readonly ConcurrentDictionary<string, bool> _keys;
        private readonly ILogger<InMemoryCacheProvider> _logger;

        public OldInMemoryCacheProvider(
                                     IMemoryCache cache,
                                     ILogger<InMemoryCacheProvider> logger
                                    )
        {
            _cache = cache;
            _keys = new ConcurrentDictionary<string, bool>();
            _logger = logger;
        }

        private int ExpirationSeconds => 600;

        private bool LoggingEnabled => false;

        public void Set<T>(string key, T value)
            where T : class
        {
            if (isValid(key, value))
            {

                Set(key, value, TimeSpan.FromSeconds(ExpirationSeconds));
            }
        }

        public void Set<T>(string key, T value, TimeSpan expiration)
            where T : class
        {
            if (isValid(key, value))
            {
                if (LoggingEnabled)
                {
                    _logger?.LogDebug($"Setting Cache = {key}");
                }
                _cache.Set(key, value, GetMemoryCacheEntryOptions(expiration));
                _keys.TryAdd(key, true);
            }
        }

        public T Get<T>(string key)
            where T : class
        {
            if (isValid(key))
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
            }
            return default;
        }

        public T Get<T>(string key, Func<T> acquire)
            where T : class
        {
            if (isValid(key))
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

                _cache.Set(key, result, GetMemoryCacheEntryOptions(TimeSpan.FromSeconds(ExpirationSeconds)));
                _keys.TryAdd(key, true);

                return result;
            }
            else
            {
                return null;
            }
        }

        public void Remove(string key)
        {
            if (isValid(key))
            {
                _cache.Remove(key);
                _keys.TryGetValue(key, out _);
            }
        }

        public void RemovePattern(string pattern)
        {

            var matchesKeys = _keys.Select(p => p.Key).Where(key => key.StartsWith(pattern)).ToList();

            foreach (var key in matchesKeys)
            {
                Remove(key);
            }
        }

        public bool Exists(string key)
        {
            bool value = false;

            if (isValid(key))
            {
                value = _cache.TryGetValue(key, out _);
            }
            return value;
        }

        public void Refresh<T>(string key, T value)
            where T : class
        {
            if (isValid(key, value))
            {
                Remove(key);
                Set(key, value, TimeSpan.FromSeconds(ExpirationSeconds));
            }
        }

        public void Refresh<T>(string key, T value, TimeSpan expiration)
            where T : class
        {
            if (isValid(key, value))
            {
                Remove(key);
                Set(key, value, expiration);
            }
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

        private void PostEviction(object key, object value, EvictionReason reason, object state)
        {
            if (reason == EvictionReason.Replaced)
            {
                return;
            }

            ClearKeys();

            TryRemoveKey(key.ToString());
        }

        private void ClearKeys()
        {
            foreach (var key in _keys.Where(p => !p.Value).Select(p => p.Key).ToList())
            {
                RemoveKey(key);
            }
        }

        private string RemoveKey(string key)
        {
            if (isValid(key))
            {
                TryRemoveKey(key);
            }
            return key;
        }

        private void TryRemoveKey(string key)
        {
            if (isValid(key))
            {
                if (!_keys.TryRemove(key, out _))
                {
                    _keys.TryUpdate(key, false, true);
                }
            }
        }
        [ExcludeFromCodeCoverage]
        private MemoryCacheEntryOptions GetMemoryCacheEntryOptions(TimeSpan cacheTime)
        {
            var options = new MemoryCacheEntryOptions()
                .AddExpirationToken(new CancellationChangeToken(_resetCacheToken.Token))
                .RegisterPostEvictionCallback(PostEviction);

            options.AbsoluteExpirationRelativeToNow = cacheTime;

            return options;
        }

        public bool isValid(string key, object value)
        {

            if (String.IsNullOrEmpty(key) && value != null)
            {
                _logger?.LogError("Key and value cannot be null! Session value not set.");
                return false;
            }
            if (string.IsNullOrEmpty(key) == false && value == null)
            {
                _logger?.LogError("Key is not null, Value is null ", new object[] { key });
                return true;
            }
            else
            {
                return true;
            }
        }

        public bool isValid(string key)
        {
            if (String.IsNullOrEmpty(key))
            {
                _logger?.LogError("Key cannot be null! Session value cannot get its value.");
                return false;
            }
            else
            {
                return true;
            }
        }
    }

}