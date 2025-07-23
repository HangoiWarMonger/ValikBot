using Bot.Application.Common.Dto;
using DSharpPlus.Entities;

namespace Bot.Discord.Common.Graphics.Embed;

/// <summary>
/// Набор вспомогательных методов для создания Embed сообщений.
/// </summary>
public static class Embed
{
    private const char EmptyChar = '\u2000';

    /// <summary>
    /// Создаёт Embed с информацией о добавленном треке.
    /// </summary>
    public static DiscordEmbed TrackQueued(TrackInfoDto trackInfo, DiscordMember member)
    {
        return new DiscordEmbedBuilder
            {
                Color = DiscordColor.Green
            }
            .WithDescription($"""
                            ### Название: 
                            [{trackInfo.Title}]({trackInfo.Url})
                            """)
            .AddField("Длительность", trackInfo.Duration?.ToString(@"hh\:mm\:ss") ?? "Неизвестна", inline: true)
            .WithThumbnail(trackInfo.ThumbnailUrl)
            .WithFooter($"От: {member.DisplayName}", member.AvatarUrl);
    }

    /// <summary>
    /// Embed при пропуске трека.
    /// </summary>
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

    /// <summary>
    /// Ошибка о том, что пользователь не в голосовом канале.
    /// </summary>
    public static DiscordEmbed UserNotInVoiceChannelError(DiscordMember member)
    {
        return Error(member, "Вы не в голосовом канале!");
    }

    /// <summary>
    /// Ошибка пустой очереди треков.
    /// </summary>
    public static DiscordEmbed TrackQueueIsEmptyError(DiscordMember member)
    {
        return Error(member, "Нет треков в очереди!");
    }

    /// <summary>
    /// Информационное сообщение.
    /// </summary>
    public static DiscordEmbed Info(string info)
    {
        return new DiscordEmbedBuilder()
            .WithDescription(info)
            .WithColor(DiscordColor.DarkGray)
            .Build();
    }

    /// <summary>
    /// Сообщение об ошибке.
    /// </summary>
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