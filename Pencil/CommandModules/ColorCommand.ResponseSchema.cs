using System.Text.Json.Serialization;

namespace Pencil.CommandModules;

internal sealed partial class ColorCommand
{
    public class Hex
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("clean")]
        public string Clean { get; set; }
    }

    public class Fraction
    {
        [JsonPropertyName("r")]
        public double R { get; set; }

        [JsonPropertyName("g")]
        public double G { get; set; }

        [JsonPropertyName("b")]
        public double B { get; set; }

        [JsonPropertyName("h")]
        public double H { get; set; }

        [JsonPropertyName("s")]
        public double S { get; set; }

        [JsonPropertyName("l")]
        public double L { get; set; }

        [JsonPropertyName("v")]
        public double V { get; set; }

        [JsonPropertyName("k")]
        public double K { get; set; }

        [JsonPropertyName("X")]
        public double X { get; set; }

        [JsonPropertyName("Y")]
        public double Y { get; set; }

        [JsonPropertyName("Z")]
        public double Z { get; set; }
    }

    public class Rgb
    {
        [JsonPropertyName("fraction")]
        public Fraction Fraction { get; set; }

        [JsonPropertyName("r")]
        public int R { get; set; }

        [JsonPropertyName("g")]
        public int G { get; set; }

        [JsonPropertyName("b")]
        public int B { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }

    public class Hsl
    {
        [JsonPropertyName("fraction")]
        public Fraction Fraction { get; set; }

        [JsonPropertyName("h")]
        public int H { get; set; }

        [JsonPropertyName("s")]
        public int S { get; set; }

        [JsonPropertyName("l")]
        public int L { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }

    public class Hsv
    {
        [JsonPropertyName("fraction")]
        public Fraction Fraction { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("h")]
        public int H { get; set; }

        [JsonPropertyName("s")]
        public int S { get; set; }

        [JsonPropertyName("v")]
        public int V { get; set; }
    }

    public class Name
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("closest_named_hex")]
        public string ClosestNamedHex { get; set; }

        [JsonPropertyName("exact_match_name")]
        public bool ExactMatchName { get; set; }

        [JsonPropertyName("distance")]
        public int Distance { get; set; }
    }

    public class Cmyk
    {
        [JsonPropertyName("fraction")]
        public Fraction Fraction { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("c")]
        public int C { get; set; }

        [JsonPropertyName("m")]
        public int M { get; set; }

        [JsonPropertyName("y")]
        public int Y { get; set; }

        [JsonPropertyName("k")]
        public int K { get; set; }
    }

    public class XYZ
    {
        [JsonPropertyName("fraction")]
        public Fraction Fraction { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("X")]
        public int X { get; set; }

        [JsonPropertyName("Y")]
        public int Y { get; set; }

        [JsonPropertyName("Z")]
        public int Z { get; set; }
    }

    public class Image
    {
        [JsonPropertyName("bare")]
        public string Bare { get; set; }

        [JsonPropertyName("named")]
        public string Named { get; set; }
    }

    public class Contrast
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }

    public class Self
    {
        [JsonPropertyName("href")]
        public string Href { get; set; }
    }

    public class Links
    {
        [JsonPropertyName("self")]
        public Self Self { get; set; }
    }

    public class Embedded
    {
    }

    public class Response
    {
        [JsonPropertyName("hex")]
        public Hex Hex { get; set; }

        [JsonPropertyName("rgb")]
        public Rgb Rgb { get; set; }

        [JsonPropertyName("hsl")]
        public Hsl Hsl { get; set; }

        [JsonPropertyName("hsv")]
        public Hsv Hsv { get; set; }

        [JsonPropertyName("name")]
        public Name Name { get; set; }

        [JsonPropertyName("cmyk")]
        public Cmyk Cmyk { get; set; }

        [JsonPropertyName("XYZ")]
        public XYZ XYZ { get; set; }

        [JsonPropertyName("image")]
        public Image Image { get; set; }

        [JsonPropertyName("contrast")]
        public Contrast Contrast { get; set; }

        [JsonPropertyName("_links")]
        public Links Links { get; set; }

        [JsonPropertyName("_embedded")]
        public Embedded Embedded { get; set; }
    }
}
