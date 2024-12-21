namespace Bot.Infrastructure.Ffmpeg;

public class FfmpegOptions
{
    public const string SectionName = "Ffmpeg";

    public string BinaryPath { get; set; } = null!;
}