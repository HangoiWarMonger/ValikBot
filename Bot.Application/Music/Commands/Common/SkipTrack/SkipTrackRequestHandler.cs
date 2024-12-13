using Ardalis.GuardClauses;
using Bot.Application.Common.Interfaces;
using Bot.Domain.Entities;
using MediatR;

namespace Bot.Application.Music.Commands.Common.SkipTrack;

public class SkipTrackRequestHandler : IRequestHandler<SkipTrackRequest>
{
    private readonly IFactory<TrackQueue, ulong> _trackQueueFactory;

    public SkipTrackRequestHandler(IFactory<TrackQueue, ulong> trackQueueFactory)
    {
        _trackQueueFactory = trackQueueFactory;
    }

    public async Task Handle(SkipTrackRequest request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Default(request.GuildId, nameof(request.GuildId));

        var queue = _trackQueueFactory.Get(request.GuildId);

        await queue.SkipAsync();
    }
}