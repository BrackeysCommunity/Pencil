using System.Text.Json.Serialization;

namespace Pencil.Data;

internal class Links
{
    [JsonPropertyName("self")] public Self Self { get; set; } = new();
}
