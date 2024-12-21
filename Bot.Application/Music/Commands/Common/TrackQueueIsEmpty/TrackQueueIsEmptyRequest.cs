using MediatR;

namespace Bot.Application.Music.Commands.Common.TrackQueueIsEmpty;

/// <summary>
/// Запрос-проверка, пуста ли очередь треков.
/// </summary>
public class TrackQueueIsEmptyRequest : IRequest<bool>
{
    /// <summary>
    /// Идентификатор гильдии.
    /// </summary>
    public ulong GuildId { get; init; }

    /// <summary>
    /// Запрос-проверка, пуста ли очередь треков.
    /// </summary>
    public TrackQueueIsEmptyRequest(ulong guildId)
    {
        GuildId = guildId;
    }
}