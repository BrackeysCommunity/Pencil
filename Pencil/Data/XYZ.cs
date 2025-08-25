using System.Text.Json.Serialization;

namespace Pencil.Data;

internal class XYZ
{
    [JsonPropertyName("fraction")] public Fraction Fraction { get; set; } = new();

    [JsonPropertyName("value")] public string Value { get; set; } = string.Empty;

    [JsonPropertyName("X")] public int X { get; set; }

    [JsonPropertyName("Y")] public int Y { get; set; }

    [JsonPropertyName("Z")] public int Z { get; set; }
}
