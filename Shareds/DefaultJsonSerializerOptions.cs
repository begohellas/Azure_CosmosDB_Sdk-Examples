using System.Text.Json;

namespace Shareds;
public static class DefaultJsonSerializerOptions
{
    private static readonly Lazy<JsonSerializerOptions> _serializerOptions = new(() => new JsonSerializerOptions(JsonSerializerDefaults.Web) { WriteIndented = true });

    public static JsonSerializerOptions Instance { get; }

    static DefaultJsonSerializerOptions()
    {
        Instance = _serializerOptions.Value;
    }
}
