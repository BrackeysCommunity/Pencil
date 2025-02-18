using System.Text.Json.Serialization;

namespace Pencil.Data;

internal sealed class ColorNameResponseSchema
{
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;

    [JsonPropertyName("r")] public int R { get; set; }

    [JsonPropertyName("g")] public int G { get; set; }

    [JsonPropertyName("b")] public int B { get; set; }
}
