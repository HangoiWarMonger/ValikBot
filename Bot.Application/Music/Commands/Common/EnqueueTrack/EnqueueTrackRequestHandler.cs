using Ardalis.GuardClauses;
using Bot.Application.Common.Interfaces;
using Bot.Domain.Entities;
using MediatR;

namespace Bot.Application.Music.Commands.Common.EnqueueTrack;

public class EnqueueTrackRequestHandler : IRequestHandler<EnqueueTrackRequest>
{
    private readonly IFactory<TrackQueue, ulong> _trackQueueFactory;
    private TrackQueue _trackQueue;

    public EnqueueTrackRequestHandler(IFactory<TrackQueue, ulong> trackQueueFactory)
    {
        _trackQueueFactory = trackQueueFactory;
    }

    public Task Handle(EnqueueTrackRequest request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(EnqueueTrackRequest));
        Guard.Against.NullOrWhiteSpace(request.Url, nameof(request.Url));
        Guard.Against.Default(request.GuildId, nameof(request.GuildId));

        _trackQueue = _trackQueueFactory.Get(request.GuildId);

        var track = new MusicTrack(request.Url);

        _trackQueue.Enqueue(track);

        return Task.CompletedTask;
    }
}