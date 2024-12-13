using MediatR;

namespace Bot.Application.Music.Commands.Common.SkipTrack;

public class SkipTrackRequest : IRequest
{
    /// <summary>
    /// Id гильдии.
    /// </summary>
    public ulong GuildId { get; init; }

    public SkipTrackRequest(ulong guildId)
    {
        GuildId = guildId;
    }
}