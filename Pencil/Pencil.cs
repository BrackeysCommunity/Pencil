using System.Threading.Tasks;
using BrackeysBot.API.Plugins;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using Hawkeye.API;
using Microsoft.Extensions.DependencyInjection;
using Pencil.CommandModules;
using Pencil.Services;

namespace Pencil;

[Plugin("Pencil")]
[PluginDescription("A plugin for rendering LaTeX expressions.")]
public class Pencil : MonoPlugin
{
    /// <inheritdoc />
    protected override void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(PluginManager.GetPlugin<IHawkeye>()!);
        services.AddSingleton<LatexService>();

        base.ConfigureServices(services);
    }

    /// <inheritdoc />
    protected override Task OnLoad()
    {
        Logger.Info("Registering command modules");
        CommandsNextExtension commandsNext = DiscordClient.GetCommandsNext();
        commandsNext.RegisterCommands<LatexCommand>();

        DiscordClient.GuildAvailable += DiscordClientOnGuildAvailable;

        return base.OnLoad();
    }

    private Task DiscordClientOnGuildAvailable(DiscordClient sender, GuildCreateEventArgs e)
    {
        Logger.Info($"Registering slash commands for {e.Guild}");
        SlashCommandsExtension slashCommands = DiscordClient.GetSlashCommands();
        slashCommands.RegisterCommands<LatexApplicationCommand>(e.Guild.Id);
        return slashCommands.RefreshCommands();
    }
}
