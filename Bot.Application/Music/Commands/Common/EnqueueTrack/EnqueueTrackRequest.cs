using MediatR;

namespace Bot.Application.Music.Commands.Common.EnqueueTrack;

/// <summary>
/// Добавление трека в очередь.
/// </summary>
public class EnqueueTrackRequest : IRequest
{
    /// <summary>
    /// Cсылки на треки.
    /// </summary>
    public string[] Urls { get; init; }

    /// <summary>
    /// Id гильдии.
    /// </summary>
    public ulong GuildId { get; init; }

    /// <summary>
    /// Добавление трека в очередь.
    /// </summary>
    public EnqueueTrackRequest(string[] urls, ulong guildId)
    {
        Urls = urls;
        GuildId = guildId;
    }
}