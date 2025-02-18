using System.Text.Json.Serialization;

namespace Pencil.Data;

internal class Name
{
    [JsonPropertyName("value")] public string Value { get; set; } = string.Empty;

    [JsonPropertyName("closest_named_hex")]
    public string ClosestNamedHex { get; set; } = string.Empty;

    [JsonPropertyName("exact_match_name")] public bool ExactMatchName { get; set; }

    [JsonPropertyName("distance")] public int Distance { get; set; }
}
