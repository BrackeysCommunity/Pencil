using System.Text.Json.Serialization;

namespace Pencil.Data;

internal class Hex
{
    [JsonPropertyName("value")] public string Value { get; set; }

    [JsonPropertyName("clean")] public string Clean { get; set; }
}
