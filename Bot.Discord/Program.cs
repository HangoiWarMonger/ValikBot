using Bot.Application.Common.Interfaces;
using Bot.Application.Common.Services;
using Bot.Application.Common.Types;
using Bot.Discord.Common.Bot;
using Bot.Discord.Common.DependencyInjection;
using Bot.Discord.Common.Extensions;
using Bot.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

await Host.CreateDefaultBuilder()
    .UseConsoleLifetime()
    .ConfigureHostConfiguration(x => x.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true))
    .ConfigureServices((hostContext, services) =>
    {
       Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();

        services.AddLogging(builder =>
        {
            builder.ClearProviders();

            builder.AddSerilog(dispose: true, logger: Log.Logger);
        });

        services.Configure<BotSettings>(x => hostContext.Configuration.Bind(BotSettings.SectionName, x));

        services
            .AddTransient<ITrackSourceResolver, TrackSourceResolver>()
            .AddYouTubeTrackClient(hostContext.Configuration)
            .AddFfmpeg(hostContext.Configuration)
            .AddMediatR(x =>
            {
                x.RegisterServicesFromAssembly(Bot.Application.AssemblyReference.Assembly);
            })
            .AddTransient<IFactory<ITrackClient, TrackSource>, TrackClientFactory>()
            .AddSingleton<IFactory<TrackQueue, ulong>, GuildTrackQueueFactory>()
            .AddHostedService<BotService>();
    })
    .RunConsoleAsync();