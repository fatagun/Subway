using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Cnd.Core.Serialization
{
    public class CompactSerializerSettings : JsonSerializerSettings
    {
        private readonly JsonSerializerSettings _settings;

        public CompactSerializerSettings()
        {
            _settings = new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver() { NamingStrategy = new CamelCaseNamingStrategy() }
            };
        }

        public JsonSerializerSettings GetSettings()
        {
            return _settings;
        }
    }
}
