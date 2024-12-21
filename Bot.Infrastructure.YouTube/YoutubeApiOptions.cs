namespace Bot.Infrastructure.YouTube;

public class YoutubeApiOptions
{
    public const string SectionName = "YoutubeApi";

    public string ApiKey { get; set; } = null!;

    public string YtDlpPath { get; set; } = null!;
}