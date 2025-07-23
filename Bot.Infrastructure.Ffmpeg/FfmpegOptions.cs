namespace Bot.Infrastructure.Ffmpeg;

/// <summary>
/// Параметры работы с ffmpeg.
/// </summary>
public class FfmpegOptions
{
    /// <summary>
    /// Название секции в конфигурации.
    /// </summary>
    public const string SectionName = "Ffmpeg";

    /// <summary>
    /// Путь к исполняемому файлу ffmpeg.
    /// </summary>
    public string BinaryPath { get; set; } = null!;
}