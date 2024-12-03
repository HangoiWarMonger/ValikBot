using System.Collections.Concurrent;

namespace Bot.Domain.Entities;

public class TrackQueue : BaseEntity<TrackQueue>
{
    private readonly ConcurrentQueue<MusicTrack> _playingQueue;
    private CancellationTokenSource _cancellationTokenSource;
    private MusicTrack? _currentSong;

    public bool IsPlaying { get; set; }
    public CancellationToken CancellationToken => _cancellationTokenSource.Token;

    public TrackQueue()
    {
        _playingQueue = new ConcurrentQueue<MusicTrack>();
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public void Enqueue(MusicTrack track)
    {
        _playingQueue.Enqueue(track);
    }

    public bool TryDequeue(out MusicTrack? track)
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

    public IEnumerable<MusicTrack> GetAll()
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