using Bot.Application.Common.Interfaces;
using Bot.Domain.Validation;
using Bot.Infrastructure.Ffmpeg;
using Bot.Infrastructure.YouTube;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bot.Discord.Common.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddYouTubeTrackClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<YoutubeApiOptions>(apiOptions =>
            {
                configuration.Bind(YoutubeApiOptions.SectionName, apiOptions);
                apiOptions.YtDlpPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "yt-dlp");

                ThrowIf.Null(apiOptions, nameof(apiOptions));
                ThrowIf.NullOrWhiteSpace(apiOptions.ApiKey, nameof(apiOptions.ApiKey));
            })
            .AddTransient<ITrackClient, YouTubeTrackClient>()
            .AddTransient<YouTubeTrackClient>();

        return services;
    }

    public static IServiceCollection AddFfmpeg(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<FfmpegOptions>(options =>
            {
                configuration.Bind(FfmpegOptions.SectionName, options);
                options.BinaryPath = "/usr/bin";

                ThrowIf.Null(options, nameof(options));
                ThrowIf.NullOrWhiteSpace(options.BinaryPath, nameof(options.BinaryPath));
            })
            .AddTransient<IPcmAudioConverter, FfmpegPcmAudioConverter>();

        return services;
    }
}