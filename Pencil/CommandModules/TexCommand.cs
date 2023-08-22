using System.Text.RegularExpressions;
using Discord;
using Discord.Interactions;
using Pencil.Configuration;
using Pencil.Services;

namespace Pencil.CommandModules;

/// <summary>
///     Represents a class which implements the <c>Render TeX</c> command and <c>/tex</c> slash command.
/// </summary>
internal sealed class TexCommand : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ConfigurationService _configurationService;
    private readonly LatexService _latexService;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TexCommand" /> class.
    /// </summary>
    /// <param name="configurationService">The configuration service.</param>
    /// <param name="latexService">The LaTeX rendering service.</param>
    public TexCommand(ConfigurationService configurationService, LatexService latexService)
    {
        _configurationService = configurationService;
        _latexService = latexService;
    }

    [MessageCommand("Render TeX")]
    public async Task RenderTexAsync(IMessage message)
    {
        if (string.IsNullOrWhiteSpace(message.Content))
        {
            await RespondAsync("This message does not contain any content.", ephemeral: true).ConfigureAwait(false);
            return;
        }

        using LatexService.RenderResult result = _latexService.Render(message.Content);
        if (!result.Success)
        {
            var embed = new EmbedBuilder();
            embed.WithColor(Color.Red);
            embed.WithTitle("Error displaying TeX");
            embed.WithDescription($"```\n{result.ErrorMessage}\n```");

            await RespondAsync(embed: embed.Build(), ephemeral: true).ConfigureAwait(false);
            return;
        }
        
        await RespondWithFileAsync(result.ImageStream, "output.png", ephemeral: true).ConfigureAwait(false);
    }

    [SlashCommand("tex", "Renders a TeX expression.")]
    public async Task TexCommandAsync(
        [Summary("expression", "The expression to render")] string expression,
        [Summary("spoiler", "Whether to render this image as a spoiler. Defaults to false.")] bool spoiler = false)
    {
        if (Context.Guild is { } guild &&
            _configurationService.TryGetGuildConfiguration(guild, out GuildConfiguration? guildConfiguration))
        {
            foreach (string pattern in guildConfiguration.FilteredRegexes)
            {
                if (Regex.IsMatch(expression, pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase))
                {
                    await RespondAsync("The expression contains a filtered word.", ephemeral: true).ConfigureAwait(false);
                    return;
                }
            }
        }

        using LatexService.RenderResult result = _latexService.Render(expression);

        if (!result.Success)
        {
            var embed = new EmbedBuilder();
            embed.WithColor(Color.Red);
            embed.WithTitle("Error displaying TeX");
            embed.WithDescription($"```\n{result.ErrorMessage}\n```");

            await RespondAsync(embed: embed.Build(), ephemeral: true).ConfigureAwait(false);
            return;
        }

        var fileName = $"{(spoiler ? "SPOILER_" : "")}output.png";
        await RespondWithFileAsync(result.ImageStream, fileName).ConfigureAwait(false);
    }
}
