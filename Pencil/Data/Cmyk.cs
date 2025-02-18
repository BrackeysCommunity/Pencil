using System.Text.Json.Serialization;

namespace Pencil.Data;

internal class Cmyk
{
    [JsonPropertyName("fraction")] public Fraction Fraction { get; set; } = new();

    [JsonPropertyName("value")] public string Value { get; set; } = string.Empty;

    [JsonPropertyName("c")] public int? C { get; set; }

    [JsonPropertyName("m")] public int? M { get; set; }

    [JsonPropertyName("y")] public int? Y { get; set; }

    [JsonPropertyName("k")] public int? K { get; set; }
}
