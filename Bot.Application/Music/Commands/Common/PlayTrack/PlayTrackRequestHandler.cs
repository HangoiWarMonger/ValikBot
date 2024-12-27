using Ardalis.GuardClauses;
using Bot.Application.Common.Interfaces;
using Bot.Application.Music.Commands.Common.GetAudioStream;
using Bot.Domain.Entities;
using MediatR;

namespace Bot.Application.Music.Commands.Common.PlayTrack;

public class PlayTrackRequestHandler : IRequestHandler<PlayTrackRequest>
{
    private readonly IFactory<TrackQueue, ulong> _trackQueueFactory;
    private readonly IPcmAudioConverter _pcmAudioConverter;
    private readonly ISender _sender;
    private TrackQueue _trackQueue;

    public PlayTrackRequestHandler(IFactory<TrackQueue, ulong> trackQueueFactory, ISender sender, IPcmAudioConverter pcmAudioConverter)
    {
        _trackQueueFactory = trackQueueFactory;
        _sender = sender;
        _pcmAudioConverter = pcmAudioConverter;
    }

    public async Task Handle(PlayTrackRequest request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request.RestreamAction, nameof(request.RestreamAction));
        Guard.Against.Default(request.GuildId, nameof(request.GuildId));

        _trackQueue = _trackQueueFactory.Get(request.GuildId);

        if (!_trackQueue.IsPlaying)
        {
            await PlayNextInQueue(request.RestreamAction, request.EndStreamAction, request.OnNewTrackAction);
        }
    }

    private async Task PlayNextInQueue(Func<Stream, CancellationToken, Task> restreamAction, Func<Task> endStreamAction, Func<MusicTrack, Task> onNewTrackAction)
    {
        if (_trackQueue.TryDequeue(out var track))
        {
            _trackQueue.IsPlaying = true;

            try
            {
                await using var audioStream = await _sender.Send(new GetAudioStreamRequest(track!.Link.Url), _trackQueue.CancellationToken);

                await onNewTrackAction(track);
                await _pcmAudioConverter.ConvertAndStreamAsync(audioStream, restreamAction, _trackQueue.CancellationToken);
            }
            finally
            {
                _trackQueue.IsPlaying = false;
                await endStreamAction();

                await PlayNextInQueue(restreamAction, endStreamAction, onNewTrackAction);
            }
        }
    }
}