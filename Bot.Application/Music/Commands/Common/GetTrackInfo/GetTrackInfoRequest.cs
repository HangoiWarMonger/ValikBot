using Bot.Application.Common.Dto;
using MediatR;

namespace Bot.Application.Music.Commands.Common.GetTrackInfo;

/// <summary>
/// Запрос на получение информации о треке.
/// </summary>
public class GetTrackInfoRequest : IRequest<TrackInfoDto>
{
    /// <summary>
    /// Ссылка на трек.
    /// </summary>
    public string Url { get; init; }

    /// <summary>
    /// Запрос на получение информации о треке.
    /// </summary>
    public GetTrackInfoRequest(string url)
    {
        Url = url;
    }
}