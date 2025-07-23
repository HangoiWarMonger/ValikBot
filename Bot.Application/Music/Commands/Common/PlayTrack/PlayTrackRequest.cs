using Bot.Domain.Entities;
using MediatR;

namespace Bot.Application.Music.Commands.Common.PlayTrack;

/// <summary>
/// Запрос на воспроизведение трека.
/// </summary>
public class PlayTrackRequest : IRequest
{
    /// <summary>
    /// Функция рестриминга аудио потока.
    /// </summary>
    public Func<Stream, CancellationToken, Task> RestreamAction { get; init; }

    /// <summary>
    /// Функция по завершению стриминга.
    /// </summary>
    public Func<Task> EndStreamAction { get; init; }

    /// <summary>
    /// Id гильдии.
    /// </summary>
    public ulong GuildId { get; set; }

    /// <summary>
    /// Действие при проигрывании нового трека.
    /// </summary>
    public Func<MusicTrack, Task> OnNewTrackAction { get; init; }

    /// <summary>
    /// Создаёт запрос на воспроизведение трека.
    /// </summary>
    public PlayTrackRequest(Func<Stream, CancellationToken, Task> restreamAction, Func<Task> endStreamAction, ulong guildId, Func<MusicTrack, Task> onNewTrackAction)
    {
        RestreamAction = restreamAction;
        EndStreamAction = endStreamAction;
        GuildId = guildId;
        OnNewTrackAction = onNewTrackAction;
    }
}