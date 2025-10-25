using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pencil.Commands;
using Pencil.Services;
using Serilog;
using X10D.Hosting.DependencyInjection;

Directory.CreateDirectory("data");

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/latest.log", rollingInterval: RollingInterval.Day)
#if DEBUG
    .MinimumLevel.Debug()
#endif
    .CreateLogger();

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddJsonFile("data/config.json", true, true);
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

builder.Services.AddSingleton<ConfigurationService>();

const DiscordIntents intents = DiscordIntents.AllUnprivileged | DiscordIntents.GuildMessages | DiscordIntents.MessageContents;
builder.Services.AddDiscordClient(Environment.GetEnvironmentVariable("DISCORD_TOKEN")!, intents);
builder.Services.AddCommandsExtension((_, commands) =>
{
    commands.AddCommands<ColorCommand>();
    commands.AddCommands<FormatCodeCommand>();
    commands.AddCommands<InfoCommand>();
    commands.AddCommands<TexCommand>();
});

builder.Services.AddSingleton<HttpClient>();
builder.Services.AddHostedSingleton<ColorService>();
builder.Services.AddSingleton<LatexService>();
builder.Services.AddHostedSingleton<BotService>();

IHost app = builder.Build();
await app.RunAsync();
