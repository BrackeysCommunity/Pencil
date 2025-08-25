using System.Text.RegularExpressions;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Logging;
using Pencil.Configuration;
using Pencil.Services;

namespace Pencil.Commands;

/// <summary>
///     Represents a class which implements the <c>Render TeX</c> command and <c>/tex</c> slash command.
/// </summary>
internal sealed class TexCommand : ApplicationCommandModule
{
    private readonly ILogger<TexCommand> _logger;
    private readonly ConfigurationService _configurationService;
    private readonly LatexService _latexService;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TexCommand" /> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="configurationService">The configuration service.</param>
    /// <param name="latexService">The LaTeX rendering service.</param>
    public TexCommand(ILogger<TexCommand> logger, ConfigurationService configurationService, LatexService latexService)
    {
        _logger = logger;
        _configurationService = configurationService;
        _latexService = latexService;
    }

    [ContextMenu(ApplicationCommandType.MessageContextMenu, "Render TeX")]
    public async Task RenderTexAsync(ContextMenuContext context)
    {
        DiscordMessage message = context.TargetMessage;
        string content = message.Content;

        if (string.IsNullOrWhiteSpace(content))
        {
            await context.CreateResponseAsync("This message does not contain any content.", true);
            return;
        }

        _logger.LogInformation("Rendering TeX for message {Id} ({Message})", message.Id, content);
        using LatexService.RenderResult result = _latexService.Render(content);
        if (!result.Success)
        {
            var embed = new DiscordEmbedBuilder();
            embed.WithColor(DiscordColor.Red);
            embed.WithTitle("Error displaying TeX");
            embed.WithDescription($"```\n{result.ErrorMessage}\n```");

            await context.CreateResponseAsync(embed, true);
            return;
        }

        var builder = new DiscordInteractionResponseBuilder();
        builder.AsEphemeral();
        builder.AddFile("output.png", result.ImageStream);
        await context.CreateResponseAsync(builder);
    }

    [SlashCommand("tex", "Renders a TeX expression.")]
    public async Task TexCommandAsync(InteractionContext context,
        [Option("expression", "The expression to render")] string expression,
        [Option("spoiler", "Whether to render this image as a spoiler. Defaults to false.")] bool spoiler = false)
    {
        if (context.Guild is { } guild &&
            _configurationService.TryGetGuildConfiguration(guild, out GuildConfiguration? guildConfiguration))
        {
            foreach (string pattern in guildConfiguration.FilteredRegexes)
            {
                if (Regex.IsMatch(expression, pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase))
                {
                    await context.CreateResponseAsync("The expression contains a filtered word.", true);
                    return;
                }
            }
        }

        _logger.LogInformation("Rendering TeX for expression {Expression}", expression);
        using LatexService.RenderResult result = _latexService.Render(expression);

        if (!result.Success)
        {
            var embed = new DiscordEmbedBuilder();
            embed.WithColor(DiscordColor.Red);
            embed.WithTitle("Error displaying TeX");
            embed.WithDescription($"```\n{result.ErrorMessage}\n```");

            await context.CreateResponseAsync(embed, true);
            return;
        }

        var fileName = $"{(spoiler ? "SPOILER_" : "")}output.png";
        var builder = new DiscordInteractionResponseBuilder();
        builder.AsEphemeral();
        builder.AddFile(fileName, result.ImageStream);
        await context.CreateResponseAsync(builder);
    }
}
