using Bot.Application.Common.Dto;

namespace Bot.Application.Common.Interfaces;

/// <summary>
/// Контракт работы клиента, работающего с треками.
/// </summary>
public interface ITrackClient
{
    /// <summary>
    /// Получение аудио потока по ссылке.
    /// </summary>
    /// <param name="videoUrl">Ссылка.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task<Stream> GetAudioStreamAsync(string videoUrl, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получение данных о треке по ссылке.
    /// </summary>
    /// <param name="url">Ссылка.</param>
    /// <returns>Информация о треке.</returns>
    Task<TrackInfoDto> GetInfoAsync(string url);

    /// <summary>
    /// Получение списка треков из ссылки.
    /// <remarks>
    /// Если ссылка на одиночный трек, то возвращает его же,
    /// Если ссылка на плейлист, то возвращается список треков.
    /// </remarks>
    /// </summary>
    /// <param name="requestRequestText"></param>
    /// <returns></returns>
    Task<string[]> GetTracksFromLink(string requestRequestText);
}