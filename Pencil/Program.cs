using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Pencil.Services;
using X10D.Hosting.DependencyInjection;

Directory.CreateDirectory("data");

await Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(builder => builder.AddJsonFile("data/config.json", true, true))
    .ConfigureLogging(builder =>
    {
        builder.ClearProviders();
        builder.AddNLog();
    })
    .ConfigureServices(services =>
    {
        services.AddSingleton<DiscordSocketClient>();
        services.AddSingleton<InteractionService>();
        services.AddSingleton(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMessages | GatewayIntents.MessageContent
        });

        services.AddSingleton<HttpClient>();

        services.AddHostedSingleton<LoggingService>();
        services.AddSingleton<LatexService>();
        services.AddSingleton<ConfigurationService>();
        services.AddHostedSingleton<BotService>();
    })
    .UseConsoleLifetime()
    .RunConsoleAsync();
