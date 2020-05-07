namespace Cnd.Cache.Redis
{
    using System.Collections.Generic;
    using StackExchange.Redis;

    public interface IRedisDatabaseProvider
    {
        IDatabase GetDatabase();
        ISubscriber GetSubscriber();

        IEnumerable<IServer> GetServers();

        string GetConnectionString();
    }
}
