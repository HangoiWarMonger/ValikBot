using System.Collections.Concurrent;

namespace Discord.Valik.Api.Commands;

public class VoiceQueueManager : IVoiceQueueManager
{
    private readonly ConcurrentDictionary<ulong, ConcurrentQueue<string>> _queue;

    public VoiceQueueManager()
    {
        _queue = new ConcurrentDictionary<ulong, ConcurrentQueue<string>>();
    }
    
    public void Add(ulong guildId, string url)
    {
        if (!_queue.ContainsKey(guildId))
        {
            _queue[guildId] = new ConcurrentQueue<string>();
        }
        
        _queue[guildId].Enqueue(url);
    }

    public bool HasAny(ulong guildId)
    {
        return _queue.ContainsKey(guildId);
    }
}

public interface IVoiceQueueManager
{
    bool HasAny(ulong guildId);
    void Add(ulong guildId, string url);
}