using Bot.Application.Common.Interfaces;
using Bot.Application.Music.Commands.Common.ConvertAudionToPcmAndStream;
using Bot.Application.Music.Commands.Common.GetAudioStream;
using Bot.Domain.Entities;
using MediatR;

namespace Bot.Application.Music.Commands.Common.PlayTrack;

public class PlayTrackRequestHandler : IRequestHandler<PlayTrackRequest>
{
    private readonly IFactory<TrackQueue, ulong> _trackQueueFactory;
    private ISender _sender;
    private TrackQueue _trackQueue;

    public PlayTrackRequestHandler(IFactory<TrackQueue, ulong> trackQueueFactory, ISender sender)
    {
        _trackQueueFactory = trackQueueFactory;
        _sender = sender;
    }

    public async Task Handle(PlayTrackRequest request, CancellationToken cancellationToken)
    {
        _trackQueue = _trackQueueFactory.Get(request.GuildId);

        if (!_trackQueue.IsPlaying)
        {
            await PlayNextInQueue(request.RestreamAction, request.EndStreamAction);
        }
    }

    private async Task PlayNextInQueue(Func<Stream, Task> restreamAction, Task endStreamAction)
    {
        if (_trackQueue.TryDequeue(out var track))
        {
            _trackQueue.IsPlaying = true;

            try
            {
                var audioStream = await _sender.Send(new GetAudioStreamRequest(track!.Link.Url));

                var convertAndStreamRequest = new ConvertAudioToPcmAndRestreamRequest(audioStream, restreamAction);
                await _sender.Send(convertAndStreamRequest);
            }
            finally
            {
                _trackQueue.IsPlaying = false;
                await endStreamAction;

                await PlayNextInQueue(restreamAction, endStreamAction);
            }
        }
    }
}