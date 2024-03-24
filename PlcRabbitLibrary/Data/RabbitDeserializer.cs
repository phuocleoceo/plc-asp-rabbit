using System.Text;
using Newtonsoft.Json;

namespace PlcRabbitLibrary.Data;

public static class RabbitDeserializer<T>
{
    public static T Deserialize(byte[] data)
    {
        return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(data));
    }
}
