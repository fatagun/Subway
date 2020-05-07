namespace Cnd.Cache.Redis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Cnd.Core.ServiceLifetime;
    using Microsoft.Extensions.Options;
    using StackExchange.Redis;
    using Microsoft.Extensions.Logging;

    public class RedisDatabaseProvider : IRedisDatabaseProvider, ISingletonService
    {
        private readonly RedisDbOptions _options;
        private readonly Lazy<ConnectionMultiplexer> _connectionMultiplexer;
        private readonly ILogger<RedisDatabaseProvider> _logger;

        public RedisDatabaseProvider(IOptions<RedisDbOptions> options, ILogger<RedisDatabaseProvider> logger)
        {
            _logger = logger;
            _options = options.Value;
            _connectionMultiplexer = new Lazy<ConnectionMultiplexer>(CreateConnectionMultiplexer);
        }

        public IDatabase GetDatabase()
        {
            return _connectionMultiplexer.Value.GetDatabase(_options.Database);
        }

        public ISubscriber GetSubscriber()
        {
            return _connectionMultiplexer.Value.GetSubscriber();
        }

        public IEnumerable<IServer> GetServers()
        {
            var endpoints = GetMastersServersEndpoints();

            foreach (var endpoint in endpoints)
            {
                yield return _connectionMultiplexer.Value.GetServer(endpoint);
            }
        }

        public string GetConnectionString()
        {
            var configurationOptions = new ConfigurationOptions
            {
                ConnectTimeout = _options.ConnectionTimeout,
                Password = _options.Password,
                Ssl = _options.IsSsl,
                SslHost = _options.SslHost,
                CommandMap = CommandMap.Create(_options.CommandMap)
            };

            try
            {
                configurationOptions.Password = configurationOptions.Password;
            }
            catch
            {
                _logger.LogCritical("Redis Password was not encrypted!!!");
            }

            var list = _options.Servers.Distinct();
            foreach (var endpoint in list)
            {
                configurationOptions.EndPoints.Add(endpoint.Host, int.Parse(endpoint.Port));
            }

            return configurationOptions.ToString();
        }

        private ConnectionMultiplexer CreateConnectionMultiplexer() => ConnectionMultiplexer.Connect(GetConnectionString());

        private List<EndPoint> GetMastersServersEndpoints()
        {
            var masters = new List<EndPoint>();
            foreach (var ep in this._connectionMultiplexer.Value.GetEndPoints())
            {
                var server = this._connectionMultiplexer.Value.GetServer(ep);
                if (server.IsConnected)
                {
                    if (server.ServerType == ServerType.Cluster)
                    {
                        masters.AddRange(server.ClusterConfiguration.Nodes.Where(n => !n.IsSlave).Select(n => n.EndPoint));
                        break;
                    }

                    if (server.ServerType == ServerType.Standalone && !server.IsSlave)
                    {
                        masters.Add(ep);
                        break;
                    }
                }
            }

            return masters;
        }
    }
}
