using System.Text.Json.Serialization;

namespace Pencil.Data;

internal class Fraction
{
    [JsonPropertyName("r")] public double R { get; set; }

    [JsonPropertyName("g")] public double G { get; set; }

    [JsonPropertyName("b")] public double B { get; set; }

    [JsonPropertyName("h")] public double H { get; set; }

    [JsonPropertyName("s")] public double S { get; set; }

    [JsonPropertyName("l")] public double L { get; set; }

    [JsonPropertyName("v")] public double V { get; set; }

    [JsonPropertyName("k")] public double K { get; set; }

    [JsonPropertyName("X")] public double X { get; set; }

    [JsonPropertyName("Y")] public double Y { get; set; }

    [JsonPropertyName("Z")] public double Z { get; set; }
}