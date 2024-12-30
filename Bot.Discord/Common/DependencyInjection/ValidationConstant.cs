namespace Bot.Discord.Common.DependencyInjection;

/// <summary>
/// Набор констант валидации.
/// </summary>
public static class ValidationConstant
{
    /// <summary>
    /// Выражение для проверки того, что ссылка ведет на youtube видео.
    /// </summary>
    public const string YouTubeVideoUrlRegex = @"^https{0,1}:\/\/(www)?\.youtube\.com\/watch\?v=.+$";

    /// <summary>
    /// Выражение для проверки того, что ссылка ведет на SoundCloud трек.
    /// </summary>
    public const string SoundCloudTrackUrlRegex = @"^https?:\/\/(www\.)?soundcloud\.com\/[a-zA-Z0-9_-]+\/(sets\/[a-zA-Z0-9_-]+|[a-zA-Z0-9_-]+)\/?$";
}