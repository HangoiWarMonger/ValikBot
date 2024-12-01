using System.Collections.Concurrent;

namespace Discord.Valik.Api.Commands.Music;

public class TrackQueue
{
    private readonly ConcurrentQueue<string> _playingQueue;
    private CancellationTokenSource _cancellationTokenSource;
    private string? _currentSong;

    public bool IsPlaying { get; set; }
    public CancellationToken CancellationToken => _cancellationTokenSource.Token;


    public TrackQueue()
    {
        _playingQueue = new ConcurrentQueue<string>();
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public void Enqueue(string track)
    {
        _playingQueue.Enqueue(track);
    }

    public bool TryDequeue(out string? track)
    {
        var hasTrack = _playingQueue.TryDequeue(out var internalTrack);

        if (!hasTrack)
        {
            track = null;
            return false;
        }

        track = internalTrack!;
        _currentSong = internalTrack!;
        return true;
    }

    public bool Any()
    {
        return !_playingQueue.IsEmpty || _currentSong != null;
    }

    public IEnumerable<string> GetAll()
    {
        yield return _currentSong!;
        foreach (var item in _playingQueue)
        {
            yield return item;
        }
    }
    
    public async Task SkipAsync()
    {
        await _cancellationTokenSource.CancelAsync();
        _cancellationTokenSource = new CancellationTokenSource();
        IsPlaying = false;

        if (_playingQueue.IsEmpty)
        {
            _currentSong = null;
        }
    }
}