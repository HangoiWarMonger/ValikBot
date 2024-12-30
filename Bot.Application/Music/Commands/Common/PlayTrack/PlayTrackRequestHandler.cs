using Ardalis.GuardClauses;
using Bot.Application.Common.Interfaces;
using Bot.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bot.Application.Music.Commands.Common.PlayTrack;

public class PlayTrackRequestHandler : IRequestHandler<PlayTrackRequest>
{
    private readonly IFactory<TrackQueue, ulong> _trackQueueFactory;
    private readonly IFactory<ITrackClient, string> _trackClientFactory;
    private readonly ILogger<PlayTrackRequestHandler> _logger;
    private readonly IPcmAudioConverter _pcmAudioConverter;
    private TrackQueue _trackQueue;

    public PlayTrackRequestHandler(IFactory<TrackQueue, ulong> trackQueueFactory, IPcmAudioConverter pcmAudioConverter, ILogger<PlayTrackRequestHandler> logger, IFactory<ITrackClient, string> trackClientFactory)
    {
        _trackQueueFactory = trackQueueFactory;
        _pcmAudioConverter = pcmAudioConverter;
        _logger = logger;
        _trackClientFactory = trackClientFactory;
    }

    public async Task Handle(PlayTrackRequest request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request.RestreamAction, nameof(request.RestreamAction));
        Guard.Against.Default(request.GuildId, nameof(request.GuildId));

        _trackQueue = _trackQueueFactory.Get(request.GuildId);

        if (!_trackQueue.IsPlaying)
        {
            _logger.LogDebug("Музыка сейчас не играет, запускаем проигрывание");
            await PlayNextInQueue(request.RestreamAction, request.EndStreamAction, request.OnNewTrackAction);
        }
        else
        {
            _logger.LogDebug("В очереди уже есть треки. Проигрывание не запущено.");
        }
    }

    private async Task PlayNextInQueue(Func<Stream, CancellationToken, Task> restreamAction, Func<Task> endStreamAction, Func<MusicTrack, Task> onNewTrackAction)
    {
        _logger.LogDebug("Пробуем запустить следующий трек...");
        if (_trackQueue.TryDequeue(out var track))
        {
            _trackQueue.IsPlaying = true;
            _logger.LogDebug("Трек получен, запускаем проигрывание.");
            try
            {
                var trackClient = _trackClientFactory.Get(track!.Link.Url);

                await using var audioStream = await trackClient.GetAudioStreamAsync(track.Link.Url, _trackQueue.CancellationToken);
                _logger.LogDebug("Получен аудиопоток.");

                _logger.LogDebug("Выполнение действий перед проигрыванием...");
                await onNewTrackAction(track);

                _logger.LogDebug("Конвертируем и транслируем в дискорд...");
                await _pcmAudioConverter.ConvertAndStreamAsync(audioStream, restreamAction, _trackQueue.CancellationToken);
            }
            finally
            {
                _trackQueue.IsPlaying = false;
                await endStreamAction();

                await PlayNextInQueue(restreamAction, endStreamAction, onNewTrackAction);
            }
        }
        else
        {
            _logger.LogDebug("Нет треков в очереди.");
        }
    }
}