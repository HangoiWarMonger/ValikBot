using MediatR;

namespace Bot.Application.Music.Commands.Common.SkipTrack;

/// <summary>
/// Запрос на пропуск текущего трека.
/// </summary>
public class SkipTrackRequest : IRequest
{
    /// <summary>
    /// Id гильдии.
    /// </summary>
    public ulong GuildId { get; init; }

    /// <summary>
    /// Создаёт запрос на пропуск трека.
    /// </summary>
    public SkipTrackRequest(ulong guildId)
    {
        GuildId = guildId;
    }
}