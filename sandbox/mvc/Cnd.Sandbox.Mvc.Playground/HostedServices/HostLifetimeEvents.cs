using Cnd.Cache.Redis;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Cnd.Sandbox.Mvc.Playground
{
    public class HostLifetimeEvents : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly IRedisCacheProvider _redis;

        public HostLifetimeEvents(
            ILogger<HostLifetimeEvents> logger,
            IHostApplicationLifetime appLifetime,
            IRedisCacheProvider redis,
            IHostLifetime host)
        {
            _logger = logger;
            _appLifetime = appLifetime;
            _redis = redis;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _appLifetime.ApplicationStarted.Register(OnStarted);
            _appLifetime.ApplicationStopping.Register(OnStopping);
            _appLifetime.ApplicationStopped.Register(OnStopped);

            await _redis.GetSubscriber().SubscribeAsync("ConfigurationUpdate", async (channel, message) =>
            {
                if(message.HasValue)
                {
                    _logger.LogInformation("in subscription.");
                    var result = await _redis.GetStringAsync<string>("Config");
                    _logger.LogInformation($"{result}");
                }
            });
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopasync has been called.");

            return Task.CompletedTask;
        }

        private void OnStarted()
        {
            _logger.LogInformation("OnStarted has been called.");
        }

        private void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called.");
        }

        private void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");
        }
    }
}