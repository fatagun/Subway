namespace Cnd.Cache.Redis
{
    public sealed class RedisOptions
    {
        public bool LoggingEnabled { get; set; } = false;

        public int ExpirationInSeconds { get; set; } = 1800; // 30 mins

        public bool IsSecure { get; set; } = false;
    }
}
