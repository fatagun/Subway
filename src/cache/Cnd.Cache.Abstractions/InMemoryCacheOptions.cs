using System.Diagnostics.CodeAnalysis;

namespace Cnd.Cache.Abstractions
{
    public class InMemoryCacheOptions
    {
        public bool EnableLogging { get; set; } = false;
        /// <summary>
        /// Sliding expiration in seconds
        /// </summary>
        public int SlidingExpirationInSeconds { get; set; } = 300;
        /// <summary>
        /// Absolute expiration in seconds.
        /// Both absolute and sliding expiration should be set otherwise some keys will be stale and never expire.
        /// Setting both will ensure cache will be fresh.
        /// </summary>
        public int AbsoluteExpirationInSeconds { get; set; } = 900;
        /// <summary>
        /// Size of the cache
        /// </summary>
        public int CacheSize { get; set; } = 1000;

        /// <summary>
        /// Expiration scan frequency to evict expired items
        /// </summary>
        public int ExpirationScanFrequency { get; set; } = 60;

    }
}
