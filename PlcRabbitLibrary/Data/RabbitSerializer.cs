using System.Text;
using Newtonsoft.Json;

namespace PlcRabbitLibrary.Data;

public static class RabbitSerializer<T>
{
    public static byte[] Serialize(T data)
    {
        return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
    }
}
