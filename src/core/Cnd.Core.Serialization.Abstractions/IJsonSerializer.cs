using System.IO;

namespace Cnd.Core.Serialization.Abstractions
{
    public interface IJsonSerializer
    {
        string Serialize(object obj);

        string Serialize<T>(T item);

        T Deserialize<T>(string json);

        string Serialize(TextWriter writer, object obj);
    }
}
