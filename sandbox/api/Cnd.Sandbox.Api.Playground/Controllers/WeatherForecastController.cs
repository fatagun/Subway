using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cnd.Cache.Redis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Cnd.Sandbox.Api.Playground.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IRedisCacheProvider _redis;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IRedisCacheProvider redis)
        {
            _logger = logger;
            _redis = redis;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var rng = new Random();

            await _redis.GetSubscriber().PublishAsync("ConfigurationUpdate", true);

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
