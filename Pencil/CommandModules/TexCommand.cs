using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Pencil.Services;

namespace Pencil.CommandModules;

/// <summary>
///     Represents a class which implements the <c>Render TeX</c> command and <c>/tex</c> slash command.
/// </summary>
internal sealed class TexCommand : ApplicationCommandModule
{
    private readonly LatexService _latexService;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TexCommand" /> class.
    /// </summary>
    /// <param name="latexService">The LaTeX rendering service.</param>
    public TexCommand(LatexService latexService)
    {
        _latexService = latexService;
    }

    [ContextMenu(ApplicationCommandType.MessageContextMenu, "Render TeX")]
    public async Task RenderTexAsync(ContextMenuContext context)
    {
        DiscordMessage? message = context.Interaction.Data.Resolved.Messages.FirstOrDefault().Value;
        if (string.IsNullOrWhiteSpace(message?.Content))
        {
            _ = context.CreateResponseAsync("This message does not contain any content.", true);
            return;
        }

        using LatexService.RenderResult result = _latexService.Render(message.Content);
        if (!result.Success)
        {
            var embed = new DiscordEmbedBuilder();
            embed.WithColor(DiscordColor.Red);
            embed.WithTitle("Error displaying TeX");
            embed.WithDescription(Formatter.BlockCode(result.ErrorMessage));

            _ = context.CreateResponseAsync(embed, true);
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
        using LatexService.RenderResult result = _latexService.Render(expression);

        if (!result.Success)
        {
            var embed = new DiscordEmbedBuilder();
            embed.WithColor(DiscordColor.Red);
            embed.WithTitle("Error displaying TeX");
            embed.WithDescription(Formatter.BlockCode(result.ErrorMessage));

            _ = context.CreateResponseAsync(embed, true);
            return;
        }

        var builder = new DiscordInteractionResponseBuilder();
        builder.AddFile($"{(spoiler ? "SPOILER_" : "")}output.png", result.ImageStream);
        await context.CreateResponseAsync(builder);
    }
}
