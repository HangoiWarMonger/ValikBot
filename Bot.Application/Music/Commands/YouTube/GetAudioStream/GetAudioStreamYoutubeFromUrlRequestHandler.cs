using Ardalis.GuardClauses;
using Bot.Application.Common.Interfaces;
using Bot.Application.Common.Types;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Bot.Application.Music.Commands.YouTube.GetAudioStream;

/// <summary>
/// Обработчик запроса на получение аудио потока из ютуб сслыки.
/// </summary>
public class GetAudioStreamYoutubeFromUrlRequestHandler : IRequestHandler<GetAudioStreamYoutubeFromUrlRequest, Stream>
{
    private readonly ITrackClient _trackClient;

    /// <summary>
    /// Обработчик запроса на получение аудио потока из ютуб сслыки.
    /// </summary>
    public GetAudioStreamYoutubeFromUrlRequestHandler([FromKeyedServices(TrackSource.Youtube)] ITrackClient trackClient)
    {
        _trackClient = trackClient;
    }

    /// <summary>
    /// Получение аудио потока из ютуб сслыки.
    /// </summary>
    public Task<Stream> Handle(GetAudioStreamYoutubeFromUrlRequest request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        return _trackClient.GetAudioStreamAsync(request.Url, cancellationToken);
    }
}