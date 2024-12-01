namespace Discord.Valik.Api;

public class BotSettings
{
    public const string SectionName = "BotConfig";

    public string Token { get; set; } = null!;

    public string Prefix { get; set; } = null!;
}