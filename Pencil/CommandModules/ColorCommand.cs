using System.Globalization;
using Discord;
using Discord.Interactions;
using Pencil.Services;
using Color = SixLabors.ImageSharp.Color;

namespace Pencil.CommandModules;

internal sealed class ColorCommand : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ColorService _colorService;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ColorCommand" /> class.
    /// </summary>
    /// <param name="colorService">The color service.</param>
    public ColorCommand(ColorService colorService)
    {
        _colorService = colorService;
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

        if (_colorService.IsCmykColor(color))
        {
            query.Add("cmyk", color);
        }
        else if (_colorService.IsHslColor(color))
        {
            query.Add("hsl", color);
        }
        else if (_colorService.IsRgbColor(color))
        {
            query.Add("rgb", color);
        }
        else if (_colorService.IsHexColor(color))
        {
            query.Add("hex", color.TrimStart('#'));
        }
        else if (_colorService.IsNamedColor(color, out Color result))
        {
            query.Add("hex", result.ToHex()[..6]);
        }
        else
        {
            await RespondAsync("Invalid color", ephemeral: true).ConfigureAwait(false);
            return;
        }

        await DeferAsync();

        var response = await _colorService.GetColorInformation(query);

        if (response is null)
        {
            await ModifyOriginalResponseAsync(properties =>
            {
                properties.Content = "An error occurred while fetching the color information.";
            });
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
}
