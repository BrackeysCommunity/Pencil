using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using Discord;
using Discord.Interactions;
using X10D.Collections;
using Color = SixLabors.ImageSharp.Color;

namespace Pencil.CommandModules;

internal sealed partial class ColorCommand : InteractionModuleBase<SocketInteractionContext>
{
    private static readonly Dictionary<string, Color> PredefinedColors = typeof(Color)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.FieldType == typeof(Color))
        .ToDictionary(f => f.Name.ToUpperInvariant(), f => (Color)f.GetValue(null)!);

    private readonly HttpClient _httpClient;


    /// <summary>
    ///     Initializes a new instance of the <see cref="ColorCommand" /> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client.</param>
    public ColorCommand(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [SlashCommand("color", "Displays information about a colour.", runMode: RunMode.Async)]
    public async Task ColorAsync(
        [Summary("color", "The color to display. This may be hex / decimal, RGB, HSL, or CMYK.")]
        string color)
    {
        var query = new Dictionary<string, string>
        {
            ["format"] = "json"
        };

        if (IsCmykColor(color))
        {
            query.Add("cmyk", color);
        }
        else if (IsHslColor(color))
        {
            query.Add("hsl", color);
        }
        else if (IsRgbColor(color))
        {
            query.Add("rgb", color);
        }
        else if (IsHexColor(color))
        {
            query.Add("hex", color.TrimStart('#'));
        }
        else if (IsNamedColor(color, out Color result))
        {
            query.Add("hex", result.ToHex()[..6]);
        }
        else
        {
            await RespondAsync("Invalid color", ephemeral: true).ConfigureAwait(false);
            return;
        }

        await DeferAsync();

        var uri = $"https://www.thecolorapi.com/id?{query.ToGetParameters()}";
        await using Stream stream = await _httpClient.GetStreamAsync(uri);
        var response = JsonSerializer.Deserialize<Response>(stream);
        if (response is null)
        {
            // invalid color
            return;
        }

        int hex = int.Parse(response.Hex.Clean, NumberStyles.HexNumber);

        var embed = new EmbedBuilder();
        embed.WithColor((uint)hex);
        embed.WithTitle(response.Name.Value);
        embed.WithThumbnailUrl($"https://singlecolorimage.com/get/{hex:X6}/128x128");
        embed.AddField("Hex", response.Hex.Value, true);
        embed.AddField("RGB", response.Rgb.Value, true);
        embed.AddField("HSL", response.Hsl.Value, true);
        embed.AddField("CMYK", response.Cmyk.Value, true);

        if (response.Name.Distance == 0)
        {
            embed.AddField("Named", response.Name.Value, true);
        }
        else
        {
            embed.AddField("Closest Named Color", response.Name.Value, true);
            embed.AddField("Closest Named Hex", response.Name.ClosestNamedHex, true);
        }

        await ModifyOriginalResponseAsync(message => message.Embed = embed.Build()).ConfigureAwait(false);
    }

    private static bool IsNamedColor(string color, out Color result)
    {
        color = Regex.Replace(color, "\\s*", string.Empty);
        color = color.ToUpperInvariant();
        return PredefinedColors.TryGetValue(color, out result);
    }

    private static bool IsHexColor(string input)
    {
        return Regex.IsMatch(input, @"^#?(?:[0-9a-fA-F]{3}){1,2}$", RegexOptions.Compiled);
    }

    private static bool IsRgbColor(string input)
    {
        return Regex.IsMatch(input, @"^rgb\((\d{1,3}),\s*(\d{1,3}),\s*(\d{1,3})\)$", RegexOptions.Compiled);
    }

    private static bool IsHslColor(string input)
    {
        return Regex.IsMatch(input, @"^hsl\((\d{1,3}),\s*(\d{1,3})%,\s*(\d{1,3})%\)$", RegexOptions.Compiled);
    }

    private static bool IsCmykColor(string input)
    {
        return Regex.IsMatch(input, @"^cmyk\((\d{1,3}),\s*(\d{1,3}),\s*(\d{1,3}),\s*(\d{1,3})\)$", RegexOptions.Compiled);
    }
}
