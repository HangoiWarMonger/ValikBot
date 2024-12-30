using System.Text.RegularExpressions;
using Bot.Application.Common.Interfaces;
using Bot.Domain.Exceptions;
using Bot.Infrastructure.SoundCloud;
using Bot.Infrastructure.YouTube;

namespace Bot.Discord.Common.DependencyInjection;

public partial class TrackClientFactory : IFactory<ITrackClient, string>
{
    private readonly YouTubeTrackClient _youTubeTrackClient;
    private readonly SoundCloudTrackClient _soundCloudTrackClient;

    public TrackClientFactory(YouTubeTrackClient youTubeTrackClient, SoundCloudTrackClient soundCloudTrackClient)
    {
        _youTubeTrackClient = youTubeTrackClient;
        _soundCloudTrackClient = soundCloudTrackClient;
    }

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