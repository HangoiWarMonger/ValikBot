using Bot.Application.Common.Dto;
using DSharpPlus.Entities;

namespace Bot.Discord.Common.Graphics.Embed;

public static class Embed
{
    private const char EmptyChar = '\u2000';

    public static DiscordEmbed TrackQueued(TrackInfoDto trackInfo, DiscordMember member)
    {
        return new DiscordEmbedBuilder
            {
                Color = DiscordColor.Green
            }
            .WithDescription($"""
                            ### Название: 
                            "[{trackInfo.Title}]({trackInfo.Url})"
                            """)
            .AddField("Длительность", trackInfo.Duration?.ToString(@"hh\:mm\:ss") ?? "Неизвестна", inline: true)
            .WithThumbnail(trackInfo.ThumbnailUrl)
            .WithFooter($"От: {member.DisplayName}", member.AvatarUrl);
    }

    public static DiscordEmbed TrackSkip(DiscordMember member)
    {
        return new DiscordEmbedBuilder
            {
                Title = "Пропускаем трек...",
                Color = DiscordColor.Yellow
            }
            .WithFooter($"Пропущено пользователем: {member.DisplayName}", member.AvatarUrl)
            .WithTimestamp(DateTime.UtcNow);
    }

    public static DiscordEmbed UserNotInVoiceChannelError(DiscordMember member)
    {
        return Error(member, "Вы не в голосовом канале!");
    }

    public static DiscordEmbed TrackQueueIsEmptyError(DiscordMember member)
    {
        return Error(member, "Нет треков в очереди!");
    }

    public static DiscordEmbed Info(string info)
    {
        return new DiscordEmbedBuilder()
            .WithDescription(info)
            .WithColor(DiscordColor.DarkGray)
            .Build();
    }

    public static DiscordEmbed Error(DiscordMember member, string error)
    {
        return new DiscordEmbedBuilder
            {
                Title = "Ошибка!",
                Color = DiscordColor.IndianRed
            }
            .AddField("Сообшение", error, inline: true)
            .WithFooter($"Для {member.DisplayName}", member.AvatarUrl)
            .WithTimestamp(DateTime.UtcNow);
    }
}