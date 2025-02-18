using System.Text.Json.Serialization;

namespace Pencil.Data;

internal class Response
{
    [JsonPropertyName("hex")] public Hex Hex { get; set; } = new();

    [JsonPropertyName("rgb")] public Rgb Rgb { get; set; } = new();

    [JsonPropertyName("hsl")] public Hsl Hsl { get; set; } = new();

    [JsonPropertyName("hsv")] public Hsv Hsv { get; set; } = new();

    [JsonPropertyName("name")] public Name Name { get; set; } = new();

    [JsonPropertyName("cmyk")] public Cmyk Cmyk { get; set; } = new();

    [JsonPropertyName("XYZ")] public XYZ Xyz { get; set; } = new();

    [JsonPropertyName("image")] public Image Image { get; set; } = new();

    [JsonPropertyName("contrast")] public Contrast Contrast { get; set; } = new();

    [JsonPropertyName("_links")] public Links Links { get; set; } = new();

    [JsonPropertyName("_embedded")] public Embedded Embedded { get; set; } = new();
}
