namespace Bot.Infrastructure.YouTube;

/// <summary>
/// Настройки доступа к API YouTube.
/// </summary>
public class YoutubeApiOptions
{
    /// <summary>
    /// Название секции в конфигурации.
    /// </summary>
    public const string SectionName = "YoutubeApi";

    /// <summary>
    /// API ключ.
    /// </summary>
    public string ApiKey { get; set; } = null!;

    /// <summary>
    /// Путь к утилите yt-dlp.
    /// </summary>
    public string YtDlpPath { get; set; } = null!;
}