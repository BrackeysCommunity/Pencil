using System.Text.Json.Serialization;

namespace Pencil.Data;

internal class Self
{
    [JsonPropertyName("href")] public string Href { get; set; } = string.Empty;
}
