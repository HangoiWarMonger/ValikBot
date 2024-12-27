using Ardalis.GuardClauses;
using Bot.Application.Common.Interfaces;
using Bot.Domain.Entities;
using MediatR;

namespace Bot.Application.Music.Commands.Common.EnqueueTrack;

/// <summary>
/// Добавление трека в очередь.
/// </summary>
public class EnqueueTrackHandler : IRequestHandler<EnqueueTrackRequest>
{
    private readonly IFactory<TrackQueue, ulong> _trackQueueFactory;
    private TrackQueue _trackQueue;

    /// <summary>
    /// Добавление трека в очередь.
    /// </summary>
    public EnqueueTrackHandler(IFactory<TrackQueue, ulong> trackQueueFactory)
    {
        _trackQueueFactory = trackQueueFactory;
    }

    /// <summary>
    /// Добавление трека в очередь.
    /// </summary>
    /// <param name="request">Запрос на добавление.</param>
    /// <param name="cancellationToken">Токен для отмены операции.</param>
    public Task Handle(EnqueueTrackRequest request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(EnqueueTrackRequest));
        Guard.Against.Default(request.GuildId, nameof(request.GuildId));

        _trackQueue = _trackQueueFactory.Get(request.GuildId);

        foreach (var url in request.Urls)
        {
            var track = new MusicTrack(url);

            _trackQueue.Enqueue(track);
        }

        return Task.CompletedTask;
    }
}