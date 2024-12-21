using Ardalis.GuardClauses;
using Bot.Application.Common.Interfaces;
using Bot.Application.Common.Types;
using MediatR;

namespace Bot.Application.Music.Commands.Common.GetAudioStream;

/// <summary>
/// Получение аудио потока.
/// </summary>
public class GetAudioStreamHandler : IRequestHandler<GetAudioStreamRequest, Stream>
{
    private readonly IFactory<ITrackClient, TrackSource> _trackClientFactory;
    private readonly ITrackSourceResolver _trackSourceResolver;

    /// <summary>
    /// Получение аудио потока.
    /// </summary>
    public GetAudioStreamHandler(ITrackSourceResolver trackSourceResolver, IFactory<ITrackClient, TrackSource> trackClientFactory)
    {
        _trackSourceResolver = trackSourceResolver;
        _trackClientFactory = trackClientFactory;
    }

    /// <summary>
    /// Получение аудио потока.
    /// </summary>
    /// <param name="request">Запрос.</param>
    /// <param name="cancellationToken">Токен для отмены операции.</param>
    public Task<Stream> Handle(GetAudioStreamRequest request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.NullOrWhiteSpace(request.Url, nameof(request.Url));

        var source = _trackSourceResolver.GetTrackSource(request.Url);

        ITrackClient trackClient = _trackClientFactory.Get(source);

        return trackClient.GetAudioStreamAsync(request.Url, cancellationToken);
    }
}