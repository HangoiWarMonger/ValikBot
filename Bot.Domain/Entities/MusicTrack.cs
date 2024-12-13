using Bot.Domain.ValueObjects;

namespace Bot.Domain.Entities;

/// <summary>
/// Музыкальный трек.
/// </summary>
public class MusicTrack : BaseEntity<MusicTrack>
{
    /// <summary>
    /// Ссылка на трек.
    /// </summary>
    public TrackLink Link { get; private set; }

    /// <summary>
    /// Музыкальный трек.
    /// </summary>
    /// <param name="url">Ссылка на трек.</param>
    public MusicTrack(string url) : base(Guid.NewGuid())
    {
        Link = new TrackLink(url);
    }
}