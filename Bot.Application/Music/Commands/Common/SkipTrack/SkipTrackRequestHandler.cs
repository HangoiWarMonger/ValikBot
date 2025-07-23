using Ardalis.GuardClauses;
using Bot.Application.Common.Interfaces;
using Bot.Domain.Entities;
using MediatR;

namespace Bot.Application.Music.Commands.Common.SkipTrack;

/// <summary>
/// Обработчик запроса на пропуск трека.
/// </summary>
public class SkipTrackRequestHandler : IRequestHandler<SkipTrackRequest>
{
    private readonly IFactory<TrackQueue, ulong> _trackQueueFactory;

    /// <summary>
    /// Создаёт обработчик.
    /// </summary>
    public SkipTrackRequestHandler(IFactory<TrackQueue, ulong> trackQueueFactory)
    {
        _trackQueueFactory = trackQueueFactory;
    }

    /// <summary>
    /// Обработка запроса.
    /// </summary>
    public async Task Handle(SkipTrackRequest request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Default(request.GuildId, nameof(request.GuildId));

        var queue = _trackQueueFactory.Get(request.GuildId);

        await queue.SkipAsync();
    }
}