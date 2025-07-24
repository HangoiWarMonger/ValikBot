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

    /// <summary>
    /// Дополнительные аргументы yt-dlp, не включающие путь к cookies и ссылку на видео.
    /// </summary>
    public string YtDlpArgs { get; set; } = "-f bestaudio --no-post-overwrites --extractor-args \"youtube:player_client=tv\"";

    /// <summary>
    /// Путь к файлу cookies.
    /// </summary>
    public string CookiesPath { get; set; } = "cookies.txt";
}