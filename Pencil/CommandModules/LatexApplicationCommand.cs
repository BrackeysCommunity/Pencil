using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Hawkeye.API;
using Pencil.Services;

namespace Pencil.CommandModules;

internal sealed class LatexApplicationCommand : ApplicationCommandModule
{
    private readonly IHawkeye _hawkeye;
    private readonly LatexService _latexService;

    /// <summary>
    ///     Initializes a new instance of the <see cref="LatexApplicationCommand" /> class.
    /// </summary>
    /// <param name="hawkeye">The Hawkeye plugin instance.</param>
    /// <param name="latexService">The LaTeX rendering service.</param>
    public LatexApplicationCommand(IHawkeye hawkeye, LatexService latexService)
    {
        _hawkeye = hawkeye;
        _latexService = latexService;
    }

    [ContextMenu(ApplicationCommandType.MessageContextMenu, "Render LaTeX")]
    public async Task RenderMessageAsync(ContextMenuContext context)
    {
        DiscordMessage? message = context.Interaction.Data.Resolved.Messages.FirstOrDefault().Value;
        if (string.IsNullOrWhiteSpace(message?.Content))
        {
            _ = context.CreateResponseAsync("This message does not contain any content.", true);
            return;
        }

        if (_hawkeye.ContainsFilteredExpression(message.Content))
        {
            // this shouldn't ever execute, as a filtered message is automatically removed by Hawkeye.
            // but in the event that a bypassing role sent the message (e.g. Admin), we'll still halt the render.
            _ = context.CreateResponseAsync("This message contains a filtered expression.", true);
            return;
        }

        using LatexService.RenderResult result = _latexService.Render(message.Content);
        if (!result.Success)
        {
            var embed = new DiscordEmbedBuilder();
            embed.WithColor(DiscordColor.Red);
            embed.WithTitle("Error displaying LaTeX");
            embed.WithDescription(Formatter.BlockCode(result.ErrorMessage));

            _ = context.CreateResponseAsync(embed, true);
            return;
        }

        var builder = new DiscordInteractionResponseBuilder();
        builder.AsEphemeral();
        builder.AddFile("output.png", result.ImageStream);
        await context.CreateResponseAsync(builder);
    }

    [SlashCommand("LaTeX", "Renders a LaTeX expression.")]
    public async Task LatexCommandAsync(InteractionContext context,
        [Option("expression", "The expression to render")]
        string expression)
    {
        if (_hawkeye.ContainsFilteredExpression(expression))
        {
            // do not render filtered expressions
            _ = context.CreateResponseAsync("This input contains a filtered expression.", true);
            return;
        }

        using LatexService.RenderResult result = _latexService.Render(expression);

        if (!result.Success)
        {
            var embed = new DiscordEmbedBuilder();
            embed.WithColor(DiscordColor.Red);
            embed.WithTitle("Error displaying LaTeX");
            embed.WithDescription(Formatter.BlockCode(result.ErrorMessage));

            _ = context.CreateResponseAsync(embed, true);
            return;
        }

        var builder = new DiscordInteractionResponseBuilder();
        builder.AddFile("output.png", result.ImageStream);
        await context.CreateResponseAsync(builder);
    }
}
