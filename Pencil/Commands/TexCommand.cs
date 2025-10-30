using System.ComponentModel;
using System.Text.RegularExpressions;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Pencil.Configuration;
using Pencil.Services;

namespace Pencil.Commands;

/// <summary>
///     Represents a class which implements the <c>Render TeX</c> command and <c>/tex</c> slash command.
/// </summary>
internal sealed class TexCommand
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

    [Command("Render TeX")]
    [SlashCommandTypes(DiscordApplicationCommandType.MessageContextMenu)]
    [UsedImplicitly]
    public async Task RenderTexAsync(SlashCommandContext context, DiscordMessage message)
    {
        _logger.LogInformation("{User} requested TeX render for {Message}", context.User, message);
        string content = message.Content;

        if (string.IsNullOrWhiteSpace(content))
        {
            await context.RespondAsync("This message does not contain any content.", true);
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

            await context.RespondAsync(embed, true);
            return;
        }

        var builder = new DiscordInteractionResponseBuilder();
        builder.AsEphemeral();
        builder.AddFile("output.png", result.ImageStream!);
        await context.RespondAsync(builder);
    }

    [Command("tex")]
    [Description("Renders a TeX expression.")]
    [UsedImplicitly]
    public async Task TexCommandAsync(SlashCommandContext context,
        [Parameter("expression"), Description("The expression to render")] string expression,
        [Parameter("spoiler"), Description("Whether to render this image as a spoiler. Defaults to false.")] bool spoiler = false,
        [Parameter("mention"), Description("The user to mention.")] DiscordUser? mentionUser = null)
    {
        _logger.LogInformation("{User} rendered TeX: {Expression} (spoiler: {Spoiler})", context.User, expression, spoiler);
        if (context.Guild is { } guild &&
            _configurationService.TryGetGuildConfiguration(guild, out GuildConfiguration? guildConfiguration))
        {
            foreach (string pattern in guildConfiguration.FilteredRegexes)
            {
                if (Regex.IsMatch(expression, pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase))
                {
                    await context.RespondAsync("The expression contains a filtered word.", true);
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

            await context.RespondAsync(embed, true);
            return;
        }

        var fileName = $"{(spoiler ? "SPOILER_" : "")}output.png";
        var builder = new DiscordInteractionResponseBuilder();

        if (mentionUser is not null)
        {
            builder.WithContent(mentionUser.Mention);
            builder.AddMention(new UserMention(mentionUser));
        }

        builder.AddFile(fileName, result.ImageStream!);
        await context.RespondAsync(builder);
    }
}
