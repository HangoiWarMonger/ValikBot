using Ardalis.GuardClauses;
using Bot.Application.Common.Dto;
using Bot.Application.Common.Interfaces;
using Bot.Domain.Validation;
using YoutubeExplode;
using YoutubeExplode.Common;
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
        Guard.Against.NullOrWhiteSpace(videoUrl);

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
        Guard.Against.NullOrWhiteSpace(url);

        var youtube = VideoLibrary.YouTube.Default;

        var realVideo = await youtube.GetVideoAsync(url);

        return new TrackInfoDto
        {
            Title = realVideo.Title,
            Url = url,
            Duration = TimeSpan.FromSeconds(100),
            ThumbnailUrl = "",
        };
    }

    /// <summary>
    /// Поиск ссылки на трек по запросу.
    /// </summary>
    /// <param name="request">Запрос.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Ссылка на трек.</returns>
    public async Task<string?> SearchTrackUrlAsync(string request, CancellationToken cancellationToken = default)
    {
        var videos = await _youtubeClient.Search.GetVideosAsync(request, cancellationToken);

        var video = videos.FirstOrDefault();
        ThrowIf.Null(video, nameof(video));

        return video!.Url;
    }
}