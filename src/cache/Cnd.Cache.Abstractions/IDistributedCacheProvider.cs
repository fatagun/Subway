using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cnd.Cache.Abstractions
{
    public interface IDistributedCacheProvider: ICacheProvider
    {
        Task SetStringAsync<T>(string key, T value) where T : class;
        Task SetStringAsync<T>(string key, T value, TimeSpan expiration) where T : class;
        Task<T> GetStringAsync<T>(string key) where T : class;
        Task RemoveStringAsync(string key);
        Task SetKeysAsync<T>(List<KeyValuePair<string,T>> keyValuePairs) where T : class;
        Task<IEnumerable<T>> GetKeysAsync<T>(IEnumerable<string> keys) where T : class;
        Task RemoveKeysAsync(IEnumerable<string> keys);
    }
}
