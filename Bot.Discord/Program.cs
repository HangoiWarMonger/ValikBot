using Bot.Application.Common.Interfaces;
using Bot.Discord.Common;
using Bot.Discord.Common.Bot;
using Bot.Discord.Common.DependencyInjection;
using Bot.Domain.Entities;
using Bot.Infrastructure.YouTube;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

await Host.CreateDefaultBuilder()
    .UseConsoleLifetime()
    .ConfigureHostConfiguration(x => x.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true))
    .ConfigureServices((hostContext, services) =>
    {
        services.Configure<BotSettings>(x => hostContext.Configuration.Bind(BotSettings.SectionName, x));

        services
            .AddSingleton<ITest, Test>()
            .AddSingleton(typeof(IFactory<TrackQueue, ulong>), typeof(GuildTrackQueueFactory))
            .AddTransient<ITrackClient, YouTubeTrackClient>()
            .AddHostedService<BotService>();
    })
    .RunConsoleAsync();