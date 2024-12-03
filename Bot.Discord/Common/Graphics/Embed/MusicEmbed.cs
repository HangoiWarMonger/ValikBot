using Bot.Application.Common.Dto;
using DSharpPlus.Entities;

namespace Bot.Discord.Common.Graphics.Embed;

public static class MusicEmbed
{
    public static DiscordEmbed TrackEmbed(TrackInfoDto trackInfo, DiscordMember member)
    {
        return new DiscordEmbedBuilder
            {
                Title = "Трек добавлен в очередь!",
                Color = DiscordColor.Green
            }
            .AddField("Название", $"[{trackInfo.Title}]({trackInfo.Url})", inline: false)
            .AddField("Длительность", trackInfo.Duration?.ToString(@"hh\:mm\:ss") ?? "Неизвестна", inline: true)
            .AddField("Ссылка", trackInfo.Url, inline: false)
            .WithThumbnail(trackInfo.Url)
            .WithFooter($"Добавлено пользователем: {member.DisplayName}", member.AvatarUrl)
            .WithTimestamp(DateTime.UtcNow);
    }
}