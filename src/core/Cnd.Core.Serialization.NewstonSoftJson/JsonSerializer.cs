using System.IO;
using Newtonsoft.Json;
using JetBrains.Annotations;
using Cnd.Core.ServiceLifetime;
using Cnd.Core.Serialization.Abstractions;

namespace Cnd.Core.Serialization
{
    public class JsonSerializer : IJsonSerializer, ISingletonService
    {
        private static readonly string JsonContentType = "application/json";
        private readonly Newtonsoft.Json.JsonSerializer _serializer;
        private readonly JsonSerializerSettings _settings;

        public string ContentType { get; set; }

        public JsonSerializer(JsonSerializerSettings settings)
        {
            ContentType = JsonContentType;
            _settings = settings;

            this._serializer = new Newtonsoft.Json.JsonSerializer
            {
                MissingMemberHandling = _settings.MissingMemberHandling,
                Formatting = _settings.Formatting,
                NullValueHandling = _settings.NullValueHandling,
                DefaultValueHandling = _settings.DefaultValueHandling,
                DateFormatString = "yyyy-MM-dd HH:mm:ss.FFFFFFF"
            };
        }

        public T Deserialize<T>([NotNull]string json)
        {
            using (var stringReader = new StringReader(json))
            {
                using (var jsonTextReader = new JsonTextReader(stringReader))
                {
                    return this._serializer.Deserialize<T>(jsonTextReader);
                }
            }
        }

        public string Serialize(object obj)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var jsonTextWriter = new JsonTextWriter(stringWriter))
                {
                    this._serializer.Serialize(jsonTextWriter, obj);

                    var result = stringWriter.ToString();
                    return result;
                }
            }
        }

        public string Serialize(TextWriter writer, object obj)
        {
            using (var stringWriter = new StringWriter())
            {
                this._serializer.Serialize(writer, obj);

                var result = stringWriter.ToString();
                return result;
            }
        }

        public string Serialize<T>(T item)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var jsonTextWriter = new JsonTextWriter(stringWriter))
                {
                    this._serializer.Serialize(jsonTextWriter, item);

                    var result = stringWriter.ToString();
                    return result;
                }
            }
        }
    }
}
