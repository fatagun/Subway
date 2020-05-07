namespace Cnd.Cache.Redis
{
    using Cnd.Cache.Abstractions;
    using StackExchange.Redis;

    public interface IRedisCacheProvider : IDistributedCacheProvider
    {
        public ISubscriber GetSubscriber();
    }
}
