using System;
using System.Threading.Tasks;
using BrackeysBot.API.Plugins;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;
using Pencil.CommandModules;
using Pencil.Services;

namespace Pencil;

/// <summary>
///     Represents a class which implements the Pencil plugin.
/// </summary>
[Plugin("Pencil")]
[PluginDescription("A plugin for rendering LaTeX expressions.")]
public sealed class Pencil : MonoPlugin
{
    /// <inheritdoc />
    protected override void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<HawkeyeAdapter>();
        services.AddSingleton<LatexService>();

        base.ConfigureServices(services);
    }

    /// <inheritdoc />
    protected override Task OnLoad()
    {
        DiscordClient.GuildAvailable += DiscordClientOnGuildAvailable;
        return base.OnLoad();
    }

    private async Task DiscordClientOnGuildAvailable(DiscordClient sender, GuildCreateEventArgs e)
    {
        await sender.BulkOverwriteGuildApplicationCommandsAsync(e.Guild.Id, ArraySegment<DiscordApplicationCommand>.Empty);

        Logger.Info($"Registering slash commands for {e.Guild}");
        SlashCommandsExtension slashCommands = DiscordClient.GetSlashCommands();
        slashCommands.RegisterCommands<LatexApplicationCommand>(e.Guild.Id);
        await slashCommands.RefreshCommands();
    }
}
