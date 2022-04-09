using System.Threading.Tasks;
using BrackeysBot.API.Extensions;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Pencil.Services;

namespace Pencil.CommandModules;

internal sealed class LatexCommand : BaseCommandModule
{
    private readonly HawkeyeAdapter _hawkeyeAdapter;
    private readonly LatexService _latexService;

    /// <summary>
    ///     Initializes a new instance of the <see cref="LatexCommand" /> class.
    /// </summary>
    /// <param name="hawkeyeAdapter">The Hawkeye plugin adapter.</param>
    /// <param name="latexService">The LaTeX rendering service.</param>
    public LatexCommand(HawkeyeAdapter hawkeyeAdapter, LatexService latexService)
    {
        _hawkeyeAdapter = hawkeyeAdapter;
        _latexService = latexService;
    }

    [Command("math")]
    [Aliases("maths", "tex", "latex", "eq", "equation")]
    [Description("Renders a LaTeX expression.")]
    public async Task LatexCommandAsync(CommandContext context,
        [Description("The expression to render."), RemainingText]
        string expression)
    {
        if (_hawkeyeAdapter.ContainsFilteredExpression(expression))
        {
            // do not render filtered expressions
            _ = context.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("🛑"));
            return;
        }

        _ = context.AcknowledgeAsync();
        using LatexService.RenderResult result = _latexService.Render(expression);

        if (!result.Success)
        {
            var embed = new DiscordEmbedBuilder();
            embed.WithColor(DiscordColor.Red);
            embed.WithTitle("Error displaying LaTeX");
            embed.WithDescription(Formatter.BlockCode(result.ErrorMessage));
            await context.RespondAsync(embed);
            return;
        }

        var builder = new DiscordMessageBuilder();
        builder.WithFile("output.png", result.ImageStream);
        await context.RespondAsync(builder);
    }
}
