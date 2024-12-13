using MediatR;

namespace Bot.Application.Music.Commands.Common.PlayTrack;

public class PlayTrackRequest : IRequest
{
    /// <summary>
    /// Функция рестриминга аудио потока.
    /// </summary>
    public Func<Stream, CancellationToken, Task> RestreamAction { get; init; }

    /// <summary>
    /// Функция по завершению стриминга.
    /// </summary>
    public Task EndStreamAction { get; init; }

    /// <summary>
    /// Id гильдии.
    /// </summary>
    public ulong GuildId { get; set; }

    public PlayTrackRequest( Func<Stream, CancellationToken, Task> restreamAction, Task endStreamAction, ulong guildId)
    {
        RestreamAction = restreamAction;
        EndStreamAction = endStreamAction;
        GuildId = guildId;
    }
}