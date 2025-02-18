using System.Text.Json.Serialization;

namespace Pencil.Data;

internal class Rgb
{
    [JsonPropertyName("fraction")] public Fraction Fraction { get; set; }

    [JsonPropertyName("r")] public int R { get; set; }

    [JsonPropertyName("g")] public int G { get; set; }

    [JsonPropertyName("b")] public int B { get; set; }

    [JsonPropertyName("value")] public string Value { get; set; }
}