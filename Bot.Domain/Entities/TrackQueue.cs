using System.Collections.Concurrent;

namespace Bot.Domain.Entities;

/// <summary>
/// Очередь треков.
/// </summary>
public class TrackQueue : BaseEntity<TrackQueue>
{
    private readonly ConcurrentQueue<MusicTrack> _playingQueue;
    private CancellationTokenSource _cancellationTokenSource;
    private MusicTrack? _currentSong;

    /// <summary>
    /// Играет ли сейчас трек.
    /// </summary>
    public bool IsPlaying { get; set; }

    /// <summary>
    /// Токен отмены текущей операции.
    /// </summary>
    public CancellationToken CancellationToken => _cancellationTokenSource.Token;

    /// <summary>
    /// Очередь треков.
    /// </summary>
    public TrackQueue(): base(Guid.NewGuid())
    {
        _playingQueue = new ConcurrentQueue<MusicTrack>();
        _cancellationTokenSource = new CancellationTokenSource();
    }

    /// <summary>
    /// Добавить трек в очередь.
    /// </summary>
    /// <param name="track">Трек.</param>
    public void Enqueue(MusicTrack track)
    {
        _playingQueue.Enqueue(track);
    }

    /// <summary>
    /// Забрать трек из очереди.
    /// </summary>
    /// <param name="track">Трек.</param>
    /// <returns>Получилось ли забрать трек.</returns>
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

    /// <summary>
    /// Если ли сейчас треки. В очереди и текущий трек.
    /// </summary>
    public bool Any()
    {
        return !_playingQueue.IsEmpty || _currentSong != null;
    }

    /// <summary>
    /// Получить все треки.
    /// </summary>
    public IEnumerable<MusicTrack> GetAll()
    {
        yield return _currentSong!;

        foreach (var item in _playingQueue)
        {
            yield return item;
        }
    }

    /// <summary>
    /// Пропустить трек.
    /// </summary>
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