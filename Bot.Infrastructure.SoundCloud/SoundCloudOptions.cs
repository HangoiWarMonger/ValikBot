namespace Bot.Infrastructure.SoundCloud;

/// <summary>
/// Настройки доступа к SoundCloud.
/// </summary>
public class SoundCloudOptions
{
    /// <summary>
    /// Название секции в конфигурации.
    /// </summary>
    public const string SectionName = "SoundCloud";

    /// <summary>
    /// Идентификатор клиента.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;
}