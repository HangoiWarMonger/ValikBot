namespace Bot.Application.Common.Dto;

/// <summary>
/// Информация о треке.
/// </summary>
public class TrackInfoDto
{
    /// <summary>
    /// Ссылка на трек.
    /// </summary>
    public string Url { get; init; }

    /// <summary>
    /// Название.
    /// </summary>
    public string Title { get; init; }

    /// <summary>
    /// Длительность.
    /// </summary>
    public TimeSpan? Duration { get; set; }

    /// <summary>
    /// Ссылка на значок видео.
    /// </summary>
    public string ThumbnailUrl { get; init; }
    
    
}