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
    /// Логин от аккаунта YouTube.
    /// </summary>
    public string Login { get; set; } = null!;

    /// <summary>
    /// Пароль от аккаунта YouTube.
    /// </summary>
    public string Password { get; set; } = null!;
}
