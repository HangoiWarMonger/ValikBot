namespace Bot.Domain.Validation;

/// <summary>
/// Набор констант валидации.
/// </summary>
public static class ValidationConstant
{
    /// <summary>
    /// Выражение для проверки того, что ссылка ведет на youtube видео.
    /// </summary>
    public const string YouTubeVideoUrlRegex = @"^https{0,1}:\/\/(www)?\.youtube\.com\/watch\?v=.+$";
}