using System.Text;
using PlcRabbitLibrary.Utils;

namespace PlcRabbitLibrary.Data;

public static class RabbitDeserializer<T>
{
    public static T Deserialize(byte[] data)
    {
        return JsonUtils.Deserialize<T>(Encoding.UTF8.GetString(data));
    }
}
