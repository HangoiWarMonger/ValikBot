namespace Bot.Discord.Common.Bot;

/// <summary>
/// Настройки дискорд бота.
/// </summary>
public class BotSettings
{
    /// <summary>
    /// Название секции в конфигурации.
    /// </summary>
    public const string SectionName = "BotConfig";

    /// <summary>
    /// Токен бота.
    /// </summary>
    public string Token { get; set; } = null!;

    /// <summary>
    /// Префикс команды в дискорде.
    /// </summary>
    public string Prefix { get; set; } = null!;
}