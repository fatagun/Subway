using System;

namespace Cnd.Cache.Abstractions
{
    public interface ICacheProvider
    {
        /// <summary>
        /// Allows setting a cache item
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        void Set<T>(string key, T value) where T : class;

        /// <summary>
        /// Allows setting a cache item with expiration
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="expiration">expiration</param>
        void Set<T>(string key, T value, TimeSpan slidingExpiration, TimeSpan absoluteExpiration) where T : class;

        /// <summary>
        /// Returns a cache item
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="key">key</param>
        /// <returns>Type</returns>
        T Get<T>(string key) where T : class;

        /// <summary>
        /// Return a cache item. If it's not cached yet, cache it
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="key">key</param>
        /// <param name="acquire">acquire</param>
        /// <returns>Type</returns>
        T Get<T>(string key, Func<T> acquire) where T : class;

        /// <summary>
        /// Removes from cache
        /// </summary>
        /// <param name="key">key</param>
        void Remove(string key);

        /// <summary>
        /// Removes from cache by pattern
        /// </summary>
        /// <param name="pattern">remove cache pattern</param>
        void RemovePattern(string pattern);

        /// <summary>
        /// Checks whether an object with key exists in the cache
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>bool</returns>
        bool Exists(string key);

        /// <summary>
        /// Refreshes an object in the cache
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        void Refresh<T>(string key, T value) where T : class;

        /// <summary>
        /// Refreshes an object with timespan
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="expiration">expiration</param>
        void Refresh<T>(string key, T value, TimeSpan expiration) where T : class;

        /// <summary>
        /// Flushes cache
        /// </summary>
        void Flush();

        /// <summary>
        /// Gets the cache item count
        /// </summary>
        /// <returns></returns>
        int Count();
    }
}
