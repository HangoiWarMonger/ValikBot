using System.Collections.Concurrent;

namespace Discord.Valik.Api.Commands.Music;

public static class TrackQueuePool
{
    private static readonly ConcurrentDictionary<ulong, TrackQueue> TrackQueue = new();

    public static TrackQueue GetTrackQueue(ulong guildId)
    {
        if (TrackQueue.TryGetValue(guildId, out var value)) return value;

        value = new TrackQueue();
        TrackQueue[guildId] = value;

        return value;
    }
}