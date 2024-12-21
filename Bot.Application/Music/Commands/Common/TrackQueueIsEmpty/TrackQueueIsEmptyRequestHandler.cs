using Ardalis.GuardClauses;
using Bot.Application.Common.Interfaces;
using Bot.Domain.Entities;
using MediatR;

namespace Bot.Application.Music.Commands.Common.TrackQueueIsEmpty;

public class TrackQueueIsEmptyRequestHandler : IRequestHandler<TrackQueueIsEmptyRequest, bool>
{
    private IFactory<TrackQueue, ulong> _trackQueueFactory;

    public TrackQueueIsEmptyRequestHandler(IFactory<TrackQueue, ulong> trackQueueFactory)
    {
        _trackQueueFactory = trackQueueFactory;
    }

    public Task<bool> Handle(TrackQueueIsEmptyRequest request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Default(request.GuildId, nameof(request.GuildId));

        var trackQueue = _trackQueueFactory.Get(request.GuildId);

        return Task.FromResult(!trackQueue.Any());
    }
}