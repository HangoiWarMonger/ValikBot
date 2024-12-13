using Bot.Application.Common.Interfaces;
using Bot.Application.Common.Types;
using Bot.Infrastructure.YouTube;

namespace Bot.Discord.Common.DependencyInjection;

public class TrackClientFactory : IFactory<ITrackClient, TrackSource>
{
    public ITrackClient Get(TrackSource key)
    {
        var service = key switch
        {
            TrackSource.Youtube => new YouTubeTrackClient(),
            _ => throw new ArgumentOutOfRangeException(nameof(key), key, null)
        };

        return service;
    }
}