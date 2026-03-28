using System.Text.Json;

namespace PlcRabbitLibrary.Utils;

public static class JsonUtils
{
    private static readonly JsonSerializerOptions DefaultOptions = new(JsonSerializerOptions.Web);

    public static string Serialize(object obj)
    {
        if (obj == null)
        {
            return "";
        }
        return JsonSerializer.Serialize(obj, DefaultOptions);
    }

    public static T Deserialize<T>(string objString)
    {
        if (string.IsNullOrWhiteSpace(objString))
        {
            return default;
        }
        return JsonSerializer.Deserialize<T>(objString, DefaultOptions);
    }
}
