using System.Collections.Concurrent;
using Bot.Application.Common.Interfaces;
using Bot.Domain.Entities;

namespace Bot.Discord.Common.DependencyInjection;

/// <summary>
/// Контракт получения очереди треков по Id гильдии.
/// </summary>
public class GuildTrackQueueFactory : IFactory<TrackQueue, ulong>
{
    private readonly ConcurrentDictionary<ulong, TrackQueue> _queues = new();

    /// <summary>
    /// Получение очереди треков по Id гильдии.
    /// </summary>
    /// <param name="key">Id гильдии.</param>
    /// <returns>Очередь треков.</returns>
    public TrackQueue Get(ulong key)
    {
        return _queues.GetOrAdd(key, _ => new TrackQueue());
    }

    /// <summary>
    /// Очищение очереди.
    /// </summary>
    /// <param name="key">Ключ.</param>
    public void ReCreate(ulong key)
    {
        _queues.TryRemove(key, out _);
        _queues.TryAdd(key, new TrackQueue());
    }
}