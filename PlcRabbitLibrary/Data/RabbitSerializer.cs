using System.Text;
using PlcRabbitLibrary.Utils;

namespace PlcRabbitLibrary.Data;

public static class RabbitSerializer<T>
{
    public static byte[] Serialize(T data)
    {
        return Encoding.UTF8.GetBytes(JsonUtils.Serialize(data));
    }
}
