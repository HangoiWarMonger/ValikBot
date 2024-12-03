using Bot.Application.Common.Dto;
using Bot.Application.Common.Interfaces;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace Bot.Infrastructure.YouTube;

/// <summary>
/// Контракт работы клиента, работающего с треками.
/// </summary>
public class YouTubeTrackClient : ITrackClient
{
    private readonly YoutubeClient _youtubeClient = new();

    /// <summary>
    /// Получение аудио потока по ссылке.
    /// </summary>
    /// <param name="videoUrl">Ссылка.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    public async Task<Stream> GetAudioStreamAsync(string videoUrl, CancellationToken cancellationToken = default)
    {
        var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(videoUrl, cancellationToken);
        var audioStreamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

        var audioStream = await _youtubeClient.Videos.Streams.GetAsync(audioStreamInfo, cancellationToken);

        return audioStream;
    }

    /// <summary>
    /// Получение данных о треке по ссылке.
    /// </summary>
    /// <param name="url">Ссылка.</param>
    /// <returns>Информация о треке.</returns>
    public async Task<TrackInfoDto> GetInfoAsync(string url)
    {
        var video = await _youtubeClient.Videos.GetAsync(url);

        return new TrackInfoDto
        {
            Title = video.Title,
            Url = video.Url,
            Duration = video.Duration,
            ThumbnailUrl = video.Thumbnails[0].Url,
        };
    }
}