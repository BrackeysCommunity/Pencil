using System.Text.Json.Serialization;

namespace Pencil.Data;

internal class Contrast
{
    [JsonPropertyName("value")] public string Value { get; set; } = string.Empty;
}
