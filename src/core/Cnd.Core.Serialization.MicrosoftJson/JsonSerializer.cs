using Cnd.Core.Serialization.Abstractions;
using Cnd.Core.ServiceLifetime;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace Cnd.Core.Serialization.MicrosoftJson
{
    public class JsonSerializer : IJsonSerializer, ISingletonService
    {
        private readonly ILogger<JsonSerializer> _logger;
        public JsonSerializer(ILogger<JsonSerializer> logger) => _logger = logger;

        public T Deserialize<T>([NotNull] string json)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("deserializing {json}", json);
            }

            var item = System.Text.Json.JsonSerializer.Deserialize<T>(json);

            return item;
        }

        public string Serialize([NotNull] object obj)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("serializing {obj}", obj);
            }

            var json = System.Text.Json.JsonSerializer.Serialize(obj);

            return json;
        }

        public string Serialize(TextWriter writer, object obj) => throw new NotImplementedException();

        public string Serialize<T>([NotNull] T item)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("serializing {item}", item);
            }

            return System.Text.Json.JsonSerializer.Serialize<T>(item);
        }
    }
}
