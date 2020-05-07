namespace Cnd.Cache.Redis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Cnd.Core.Serialization.Abstractions;
    using Cnd.Core.ServiceLifetime;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using StackExchange.Redis;
    using Cnd.Core.Common;

    public sealed class RedisCacheProvider : IRedisCacheProvider, ISingletonService
    {
        private readonly IDatabase _cache;
        private readonly ISubscriber _subscriber;
        private readonly IJsonSerializer _serializer;
        private readonly RedisOptions _redisOptions;
        private readonly IEnumerable<IServer> _servers;
        private readonly ILogger<RedisCacheProvider> _logger;

        public RedisCacheProvider(
                                  IRedisDatabaseProvider databaseProvider,
                                  IOptions<RedisOptions> options,
                                  IJsonSerializer serializer,
                                  ILogger<RedisCacheProvider> logger
                                  )
        {
            _serializer = serializer;
            _cache = databaseProvider.GetDatabase();
            _redisOptions = options.Value;
            _servers = databaseProvider.GetServers();
            _logger = logger;
            _subscriber = databaseProvider.GetSubscriber();;
        }
        private bool isSecure => _redisOptions.IsSecure;
        private bool LoggingEnabled => _redisOptions.LoggingEnabled;
        private int ExpirationInSeconds => _redisOptions.ExpirationInSeconds;

        public void Set<T>(string key, T value)
            where T : class
        {
            Set(key, value, TimeSpan.FromSeconds(ExpirationInSeconds));
        }

        public void Set<T>(string key, T value, TimeSpan expiration)
            where T : class
        {
            if (isValid(key, value))
            {
                if (isSecure)
                    _cache.StringSet(key, ProtectCache(_serializer.Serialize(value)), expiration);
                else
                    _cache.StringSet(key, _serializer.Serialize(value), expiration);
            }
        }

        public async Task SetStringAsync<T>(string key, T value) where T : class
        {
            if (isValid(key, value))
            {
                if (isSecure)
                    await _cache.StringSetAsync(key, ProtectCache(_serializer.Serialize(value)),expiry: TimeSpan.FromSeconds(ExpirationInSeconds)).ConfigureAwait(false);
                else
                   await _cache.StringSetAsync(key, _serializer.Serialize(value), expiry: TimeSpan.FromSeconds(ExpirationInSeconds)).ConfigureAwait(false);
            }
        }

        public async Task SetKeysAsync<T>(List<KeyValuePair<string, T>> keyValuePairs) where T : class
        {
            if (keyValuePairs.IsNotNull())
            {
                if (isSecure)
                {
                    await _cache.StringSetAsync(keyValuePairs.Select(c =>
                        new KeyValuePair<RedisKey, RedisValue>(c.Key, this.ProtectCache(this._serializer.Serialize(c.Value)))).ToArray()).ConfigureAwait(false);
                }
                else
                {
                    await _cache.StringSetAsync(keyValuePairs.Select(c =>
                        new KeyValuePair<RedisKey, RedisValue>(c.Key, this._serializer.Serialize(c.Value))).ToArray()).ConfigureAwait(false);
                }
            }
        }

        public async Task SetStringAsync<T>(string key, T value, TimeSpan expiration) where T : class
        {
            if (isValid(key, value))
            {
                if (isSecure)
                    await _cache.StringSetAsync(key, ProtectCache(_serializer.Serialize(value)), expiry: expiration).ConfigureAwait(false);
                else
                    await _cache.StringSetAsync(key, _serializer.Serialize(value), expiration).ConfigureAwait(false);
            }
        }

        private string ProtectCache(string data)
        {
            try
            {
                return data;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Data protection has not been worked properly, data couldn't be protected");
                return data;
            }
        }

        public async Task<T> GetStringAsync<T>(string key) where T : class
        {
            if (isValid(key))
            {
                var result = await _cache.StringGetAsync(key).ConfigureAwait(false);

                if (!result.IsNull)
                {
                    if (isSecure)
                    {
                        result = UnProtectCache(result);
                    }
                    if (LoggingEnabled)
                    {
                        _logger.LogInformation($"Cache hit: {key}");
                    }

                    var value = _serializer.Deserialize<T>(result);

                    return value;
                }

                if (LoggingEnabled)
                {
                    _logger.LogInformation($"Cache Miss: {key}");
                }
            }
            return default;
        }


        public async Task<IEnumerable<T>> GetKeysAsync<T>(IEnumerable<string> keys) where T : class
        {
            if (keys.IsNotNull())
            {
                var redisKeys = keys.Select(c => (RedisKey)c).ToArray();
                var results = await _cache.StringGetAsync(redisKeys).ConfigureAwait(false);

                if (results.IsNotNull())
                {
                    var response=new List<T>();
                    foreach (var result in results)
                    {
                        if (isSecure)
                        {
                            response.Add(this._serializer.Deserialize<T>(this.UnProtectCache(result)));
                        }
                        if (LoggingEnabled)
                        {
                            _logger.LogInformation($"Cache hit: {string.Join(",", keys.ToArray())}");
                        }

                        response.Add(_serializer.Deserialize<T>(result));
                    }

                    return response;
                }

                if (LoggingEnabled)
                {
                    _logger.LogInformation($"Cache Miss: {string.Join(",", keys.ToArray())}");
                }
            }
            return default;
        }

        public async Task RemoveStringAsync(string key)
        {
            if (isValid(key))
            {
                if (LoggingEnabled)
                {
                    _logger.LogInformation($"Removing key: {key}");
                }

                await _cache.KeyDeleteAsync(key).ConfigureAwait(false);
            }
        }


        public async Task RemoveKeysAsync(IEnumerable<string> keys)
        {
            if (keys.IsNotNull())
            {
                if (LoggingEnabled)
                {
                    _logger.LogInformation($"Removing keys: {string.Join(",",keys.ToArray())}");
                }
                var redisKeys = keys.Select(c => (RedisKey)c).ToArray();
                await _cache.KeyDeleteAsync(redisKeys).ConfigureAwait(false);
            }

        }

        public T Get<T>(string key, Func<T> factory)
            where T : class
        {
            if (isValid(key))
            {

                var result = _cache.StringGet(key);

                if (result.IsNull == false)
                {
                    if (isSecure)
                    {
                        result = UnProtectCache(result);
                    }
                    if (LoggingEnabled)
                    {
                        _logger.LogInformation($"Cache hit: {key}");
                    }

                    var value = _serializer.Deserialize<T>(result);

                    return value;
                }

                if (LoggingEnabled)
                {
                    _logger.LogInformation($"Cache Miss: {key}");
                }

                var item = factory?.Invoke();

                if (item == null)
                {
                    return default;
                }

                Set(key, item);

                return item;
            }
            else
            {
                return null;
            }
        }

        private RedisValue UnProtectCache(RedisValue result)
        {
            try
            {
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Data protection has not been worked properly, redis result protected but 'UnProtected' method is not working..");
                return result;
            }
        }

        public T Get<T>(string key)
            where T : class
        {
            if (isValid(key))
            {
                var result = _cache.StringGet(key);

                if (!result.IsNull)
                {
                    if (isSecure)
                    {
                        result = UnProtectCache(result);
                    }
                    if (LoggingEnabled)
                    {
                        _logger.LogInformation($"Cache hit: {key}");
                    }

                    var value = _serializer.Deserialize<T>(result);

                    return value;
                }

                if (LoggingEnabled)
                {
                    _logger.LogInformation($"Cache Miss: {key}");
                }
            }
            return default(T);
        }

        public void Remove(string key)
        {
            if (isValid(key))
            {
                if (LoggingEnabled)
                {
                    _logger.LogInformation($"Removing key: {key}");
                }

                _cache.KeyDelete(key);
            }
        }

        public bool Exists(string key)
        {
            bool value = false;
            if (isValid(key))
            {
                value = _cache.KeyExists(key);
            }
            return value;
        }

        public void Refresh<T>(string key, T value)
            where T : class
        {
            if (isValid(key, value))
            {
                Refresh(key, value, TimeSpan.FromSeconds(ExpirationInSeconds));
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

        public void RemoveAll(IEnumerable<string> keys)
        {

            if (LoggingEnabled)
            {
                _logger.LogInformation($"Removing keys: {keys}");
            }

            var redisKeys = keys.Where(k => !string.IsNullOrEmpty(k)).Select(k => (RedisKey)k).ToArray();
            if (redisKeys.Any())
            {
                _cache.KeyDelete(redisKeys);
            }
        }

        public int GetCount()
        {
            var total = 0;

            foreach (var server in _servers)
            {
                total += (int)server.DatabaseSize(_cache.Database);
            }

            return total;
        }

        public void Flush()
        {
            if (LoggingEnabled)
            {
                _logger.LogInformation("Flushing all servers");
            }

            foreach (var server in _servers)
            {
                server.FlushDatabase(_cache.Database);
            }
        }

        public void RemovePattern(string pattern)
        {
            // TODO : complete
        }

        public bool isValid(string key, object value)
        {
            if (string.IsNullOrEmpty(key) && value != null)
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

            if (string.IsNullOrEmpty(key))
            {
                _logger?.LogError("Key cannot be null! Session value cannot get its value.");
                return false;
            }
            else
            {
                return true;
            }
        }

        public void Set<T>(string key, T value, TimeSpan slidingExpiration, TimeSpan absoluteExpiration) where T : class
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public ISubscriber GetSubscriber()
        {
            return _subscriber;
        }
    }
}
