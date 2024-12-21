using Ardalis.GuardClauses;
using Bot.Application.Common.Dto;
using Bot.Application.Common.Interfaces;
using Bot.Application.Common.Types;
using MediatR;

namespace Bot.Application.Music.Commands.Common.GetTrackInfo;

/// <summary>
/// Получение информации о треке.
/// </summary>
public class GetTrackInfoHandler : IRequestHandler<GetTrackInfoRequest, TrackInfoDto>
{
    private readonly IFactory<ITrackClient, TrackSource> _trackClientFactory;
    private readonly ITrackSourceResolver _trackSourceResolver;

    /// <summary>
    /// Получение информации о треке.
    /// </summary>
    public GetTrackInfoHandler(ITrackSourceResolver trackSourceResolver, IFactory<ITrackClient, TrackSource> trackClientFactory)
    {
        _trackSourceResolver = trackSourceResolver;
        _trackClientFactory = trackClientFactory;
    }

    /// <summary>
    /// Получение информации о треке.
    /// </summary>
    /// <param name="request">Запрос.</param>
    /// <param name="cancellationToken">Токен для отмены операции.</param>
    public async Task<TrackInfoDto> Handle(GetTrackInfoRequest request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(GetTrackInfoRequest));
        Guard.Against.NullOrWhiteSpace(request.Url, nameof(GetTrackInfoRequest.Url));

        var source = _trackSourceResolver.GetTrackSource(request.Url);

        ITrackClient trackClient = _trackClientFactory.Get(source);

        var info = await trackClient.GetInfoAsync(request.Url);

        return info;
    }
}