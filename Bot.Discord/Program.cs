using Bot.Application.Common.Interfaces;
using Bot.Application.Music.Commands.Common.SkipTrack;
using Bot.Discord.Common.Bot;
using Bot.Discord.Common.DependencyInjection;
using Bot.Discord.Common.Extensions;
using Bot.Discord.Components;
using Bot.Domain.Entities;
using Bot.Infrastructure.YouTube;
using MediatR;
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

        services.AddSingleton<ComponentService>(provider =>
        {
            var service = new ComponentService();

            var sender = provider.GetRequiredService<ISender>();
            service.Register(UiComponent.SkipButton, async (_, eventArgs) =>
            {
                var skipRequest = new SkipTrackRequest(eventArgs.Guild.Id);
                await sender.Send(skipRequest);
            });

            return service;
        });
        services
            .AddYouTubeTrackClient(hostContext.Configuration)
            .AddSoundCloudTrackClient(hostContext.Configuration)
            .AddFfmpeg(hostContext.Configuration)
            .AddMediatR(x =>
            {
                x.RegisterServicesFromAssembly(Bot.Application.AssemblyReference.Assembly);
            })
            .AddTransient<IFactory<ITrackClient, string>, TrackClientFactory>()
            .AddTransient<ISearchService, YouTubeTrackClient>()
            .AddSingleton<GuildTrackQueueFactory>()
            .AddSingleton<IFactory<TrackQueue, ulong>>(provider =>
            {
                var guildTrackQueueFactory = provider.GetRequiredService<GuildTrackQueueFactory>();
                return guildTrackQueueFactory;
            })
            .AddHostedService<BotService>();
    })
    .RunConsoleAsync();