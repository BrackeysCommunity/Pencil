﻿using System.Reflection;
using DSharpPlus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Pencil.Services;

/// <summary>
///     Represents a service which manages the bot's Discord connection.
/// </summary>
internal sealed class BotService : BackgroundService
{
    private readonly ILogger<BotService> _logger;
    private readonly DiscordClient _discordClient;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BotService" /> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="discordClient">The Discord client.</param>
    public BotService(ILogger<BotService> logger, DiscordClient discordClient)
    {
        _logger = logger;
        _discordClient = discordClient;

        var attribute = typeof(BotService).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        Version = attribute?.InformationalVersion ?? "Unknown";
    }

    /// <summary>
    ///     Gets the date and time at which the bot was started.
    /// </summary>
    /// <value>The start timestamp.</value>
    public DateTimeOffset StartedAt { get; private set; }

    /// <summary>
    ///     Gets the bot version.
    /// </summary>
    /// <value>The bot version.</value>
    public string Version { get; }

    /// <inheritdoc />
    public override Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.WhenAll(_discordClient.DisconnectAsync(), base.StopAsync(cancellationToken));
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        StartedAt = DateTimeOffset.UtcNow;
        _logger.LogInformation("Pencil v{Version} is starting", Version);

        _logger.LogInformation("Connecting to Discord");
        await _discordClient.ConnectAsync();
    }
}
