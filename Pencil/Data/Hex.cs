using System.Text.Json.Serialization;

namespace Pencil.Data;

internal class Hex
{
    [JsonPropertyName("value")] public string Value { get; set; } = string.Empty;

    [JsonPropertyName("clean")] public string Clean { get; set; } = string.Empty;
}
