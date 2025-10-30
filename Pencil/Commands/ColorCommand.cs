using System.ComponentModel;
using System.Globalization;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using JetBrains.Annotations;
using Pencil.Data;
using Pencil.Services;
using Color = SixLabors.ImageSharp.Color;

namespace Pencil.Commands;

internal sealed class ColorCommand
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

    [Command("color")]
    [Description("Displays information about a colour.")]
    [UsedImplicitly]
    public async Task ColorAsync(SlashCommandContext context,
        [Parameter("color"), Description("The color to display. This may be hex / decimal, RGB, HSL, or CMYK.")] string color,
        [Parameter("mention"), Description("The user to mention.")] DiscordUser? mentionUser = null)
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
            await context.RespondAsync("Invalid color", true);
            return;
        }

        await context.DeferResponseAsync();

        var builder = new DiscordWebhookBuilder();
        Response? response = await _colorService.GetColorInformation(query);

        if (response is null)
        {
            builder.WithContent("An error occurred while fetching the color information.");
            await context.EditResponseAsync(builder);
            return;
        }

        int hex = int.Parse(response.Hex.Clean, NumberStyles.HexNumber);

        var embed = new DiscordEmbedBuilder();
        embed.WithColor(hex);
        embed.WithTitle(response.Name.Value);
        embed.WithThumbnail($"https://singlecolorimage.com/get/{hex:X6}/128x128");
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

        if (mentionUser is not null)
        {
            builder.WithContent(mentionUser.Mention);
            builder.AddMention(new UserMention(mentionUser));
        }

        builder.AddEmbed(embed);
        await context.EditResponseAsync(builder);
    }
}
