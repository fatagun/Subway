namespace Cnd.Cache.Redis
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public sealed class RedisDbOptions
    {
        public int Database { get; set; } = 0;

        public string Password { get; set; }

        public bool IsSsl { get; set; } = false;

        public string SslHost { get; set; }

        public int ConnectionTimeout { get; set; } = 5000;

        public IList<Server> Servers { get; set; } = new List<Server>() { new Server { Host = "localhost", Port = "6379" } };

        public Dictionary<string, string> CommandMap { get; set; }
    }
}
