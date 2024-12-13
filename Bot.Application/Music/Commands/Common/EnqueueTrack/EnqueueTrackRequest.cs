using MediatR;

namespace Bot.Application.Music.Commands.Common.EnqueueTrack;

/// <summary>
/// Добавление трека в очередь.
/// </summary>
public class EnqueueTrackRequest : IRequest
{
    /// <summary>
    /// Cсылка на трек.
    /// </summary>
    public string Url { get; init; }

    /// <summary>
    /// Id гильдии.
    /// </summary>
    public ulong GuildId { get; init; }

    /// <summary>
    /// Добавление трека в очередь.
    /// </summary>
    public EnqueueTrackRequest(string url, ulong guildId)
    {
        Url = url;
        GuildId = guildId;
    }
}