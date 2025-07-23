using System.Text.RegularExpressions;
using Bot.Application.Common.Interfaces;
using Bot.Domain.Exceptions;
using Bot.Infrastructure.SoundCloud;
using Bot.Infrastructure.YouTube;

namespace Bot.Discord.Common.DependencyInjection;

/// <summary>
/// Фабрика получения клиента треков по ссылке.
/// </summary>
public partial class TrackClientFactory : IFactory<ITrackClient, string>
{
    private readonly YouTubeTrackClient _youTubeTrackClient;
    private readonly SoundCloudTrackClient _soundCloudTrackClient;

    /// <summary>
    /// Создаёт фабрику клиентов.
    /// </summary>
    public TrackClientFactory(YouTubeTrackClient youTubeTrackClient, SoundCloudTrackClient soundCloudTrackClient)
    {
        _youTubeTrackClient = youTubeTrackClient;
        _soundCloudTrackClient = soundCloudTrackClient;
    }

    /// <summary>
    /// Получить клиент по ссылке.
    /// </summary>
    public ITrackClient Get(string key)
    {
        var source = GetSource(key);

        ITrackClient service = source switch
        {
            TrackSources.Youtube => _youTubeTrackClient,
            TrackSources.SoundCLoud => _soundCloudTrackClient,
            _ => throw new UnsupportedTrackSourceException()
        };

        return service;
    }

    /// <summary>
    /// Определить источник трека по ссылке.
    /// </summary>
    private static TrackSources GetSource(string key)
    {
        if (YouTubeRegex().IsMatch(key))
        {
            return TrackSources.Youtube;
        }
        else if (SoundCloudRegex().IsMatch(key))
        {
            return TrackSources.SoundCLoud;
        }

        return TrackSources.Undefined;
    }

    [GeneratedRegex(ValidationConstant.SoundCloudTrackUrlRegex)]
    private static partial Regex SoundCloudRegex();

    [GeneratedRegex(ValidationConstant.YouTubeVideoUrlRegex)]
    private static partial Regex YouTubeRegex();
}