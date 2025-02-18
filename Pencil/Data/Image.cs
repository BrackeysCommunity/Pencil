using System.Text.Json.Serialization;

namespace Pencil.Data;

internal class Image
{
    [JsonPropertyName("bare")] public string Bare { get; set; } = string.Empty;

    [JsonPropertyName("named")] public string Named { get; set; } = string.Empty;
}
