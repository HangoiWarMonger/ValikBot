using Bot.Application.Common.Interfaces;
using Bot.Application.Common.Types;
using Bot.Domain.Exceptions;
using Bot.Infrastructure.YouTube;

namespace Bot.Discord.Common.DependencyInjection;

public class TrackClientFactory : IFactory<ITrackClient, TrackSource>
{
    private readonly YouTubeTrackClient _youTubeTrackClient;

    public TrackClientFactory(YouTubeTrackClient youTubeTrackClient)
    {
        _youTubeTrackClient = youTubeTrackClient;
    }

    public ITrackClient Get(TrackSource key)
    {
        var service = key switch
        {
            TrackSource.Youtube => _youTubeTrackClient,
            _ => throw new UnsupportedTrackSourceException()
        };

        return service;
    }
}