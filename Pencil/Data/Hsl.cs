using System.Text.Json.Serialization;

namespace Pencil.Data;

internal class Hsl
{
    [JsonPropertyName("fraction")] public Fraction Fraction { get; set; } = new();

    [JsonPropertyName("h")] public int H { get; set; }

    [JsonPropertyName("s")] public int S { get; set; }

    [JsonPropertyName("l")] public int L { get; set; }

    [JsonPropertyName("value")] public string Value { get; set; } = string.Empty;
}
