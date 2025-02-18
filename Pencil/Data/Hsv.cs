using System.Text.Json.Serialization;

namespace Pencil.Data;

internal class Hsv
{
    [JsonPropertyName("fraction")] public Fraction Fraction { get; set; } = new();

    [JsonPropertyName("value")] public string Value { get; set; } = string.Empty;

    [JsonPropertyName("h")] public int H { get; set; }

    [JsonPropertyName("s")] public int S { get; set; }

    [JsonPropertyName("v")] public int V { get; set; }
}
