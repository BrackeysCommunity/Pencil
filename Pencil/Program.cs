using DSharpPlus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pencil.Services;
using Serilog;
using Serilog.Extensions.Logging;
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
builder.Services.AddSingleton(new DiscordClient(new DiscordConfiguration
{
    Token = Environment.GetEnvironmentVariable("DISCORD_TOKEN"),
    LoggerFactory = new SerilogLoggerFactory(),
    Intents = DiscordIntents.AllUnprivileged | DiscordIntents.GuildMessages | DiscordIntents.MessageContents
}));

builder.Services.AddSingleton<HttpClient>();

builder.Services.AddHostedSingleton<ColorService>();
builder.Services.AddSingleton<LatexService>();
builder.Services.AddHostedSingleton<BotService>();

IHost app = builder.Build();
await app.RunAsync();
