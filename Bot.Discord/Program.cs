using Bot.Application.Common.Interfaces;
using Bot.Application.Common.Services;
using Bot.Application.Common.Types;
using Bot.Discord.Common;
using Bot.Discord.Common.Bot;
using Bot.Discord.Common.DependencyInjection;
using Bot.Domain.Entities;
using Bot.Infrastructure.Ffmpeg;
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
            .AddTransient<ITrackSourceResolver, TrackSourceResolver>()
            .AddTransient<IPcmAudioConverter, FfmpegPcmAudioConverter>()
            .AddMediatR(x =>
            {
                x.RegisterServicesFromAssembly(Bot.Application.AssemblyReference.Assembly);
            })
            .AddTransient<IFactory<ITrackClient, TrackSource>, TrackClientFactory>()
            .AddSingleton<IFactory<TrackQueue, ulong>, GuildTrackQueueFactory>()
            .AddTransient<ITrackClient, YouTubeTrackClient>()
            .AddHostedService<BotService>();
    })
    .RunConsoleAsync();