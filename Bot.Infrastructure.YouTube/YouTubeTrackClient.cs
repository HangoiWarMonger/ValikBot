using System.Diagnostics;
using System.Text.Json;
using Ardalis.GuardClauses;
using Bot.Application.Common.Dto;
using Bot.Application.Common.Interfaces;
using Bot.Domain.Validation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bot.Infrastructure.YouTube;

/// <summary>
/// Контракт работы клиента, работающего с треками.
/// </summary>
public class YouTubeTrackClient : ITrackClient, ISearchService
{
    private const string SearchUrl = "https://www.googleapis.com/youtube/v3/search";
    private const string VideoInfoUrl = "https://www.googleapis.com/youtube/v3/videos";
    private readonly ILogger<YouTubeTrackClient> _logger;

    private readonly YoutubeApiOptions _options;

    public YouTubeTrackClient(IOptions<YoutubeApiOptions> options, ILogger<YouTubeTrackClient> logger)
    {
        _logger = logger;
        _options = Guard.Against.Null(options.Value, nameof(options.Value));
    }

    /// <summary>
    /// Получение аудио потока по ссылке.
    /// </summary>
    /// <param name="videoUrl">Ссылка.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    public Task<Stream> GetAudioStreamAsync(string videoUrl, CancellationToken cancellationToken = default)
    {
        Guard.Against.NullOrWhiteSpace(videoUrl);

        var cookiesPath = "cookies.txt";

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = _options.YtDlpPath,
            Arguments = $"-f bestaudio --no-post-overwrites --extractor-args \"youtube:player_client=tv\" --cookies {cookiesPath} --quiet -o - {videoUrl}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = Process.Start(startInfo);
        ThrowIf.Null(process, nameof(process));

        _ = Task.Run(async () =>
        {
            using var errorReader = process!.StandardError;
            while (await errorReader.ReadLineAsync(cancellationToken) is { } line)
            {
                _logger.LogError("YT-DLP Error: {ErrorLine}", line);
            }
        }, cancellationToken);

        return Task.FromResult(process!.StandardOutput.BaseStream);
    }

    /// <summary>
    /// Получение данных о треке по ссылке.
    /// </summary>
    /// <param name="url">Ссылка.</param>
    /// <returns>Информация о треке.</returns>
    public async Task<TrackInfoDto> GetInfoAsync(string url)
    {
        Guard.Against.NullOrWhiteSpace(url);

        var videoId = ExtractVideoIdFromUrl(url);
        ThrowIf.NullOrWhiteSpace(videoId, nameof(videoId));

        using var httpClient = new HttpClient();
        var requestUrl = $"{VideoInfoUrl}?id={videoId}&part=snippet,contentDetails&key={_options.ApiKey}";

        var response = await httpClient.GetStringAsync(requestUrl);
        using var jsonDoc = JsonDocument.Parse(response);

        var items = jsonDoc.RootElement.GetProperty("items");

        var snippet = items[0].GetProperty("snippet");
        var contentDetails = items[0].GetProperty("contentDetails");


        var title = snippet.GetProperty("title").GetString();
        var thumbnailUrl = snippet.GetProperty("thumbnails").GetProperty("default").GetProperty("url").GetString();
        var duration = ParseIso8601Duration(contentDetails.GetProperty("duration").GetString());

        return new TrackInfoDto
        {
            Title = title ?? "Без названия",
            Url = url,
            Duration = duration,
            ThumbnailUrl = thumbnailUrl,
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
        Guard.Against.NullOrWhiteSpace(request, nameof(request));

        using var httpClient = new HttpClient();
        var url = $"{SearchUrl}?part=snippet&maxResults=1&type=video&q={Uri.EscapeDataString(request)}&key={_options.ApiKey}";

        var response = await httpClient.GetAsync(url, cancellationToken);

        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
        var items = doc.RootElement.GetProperty("items");

        string? videoId = null;
        if (items.GetArrayLength() > 0)
        {
            var video = items[0]
                .GetProperty("id")
                .GetProperty("videoId")
                .GetString();

            videoId = video;
        }

        ThrowIf.NullOrWhiteSpace(videoId, nameof(videoId));

        return $"https://www.youtube.com/watch?v={videoId}";
    }

    /// <summary>
    /// Получение списка треков из ссылки.
    /// <remarks>
    /// Если ссылка на одиночный трек, то возвращает его же,
    /// Если ссылка на плейлист, то возвращается список треков.
    /// </remarks>
    /// </summary>
    /// <param name="requestRequestText"></param>
    /// <returns></returns>
    public async Task<string[]> GetTracksFromLink(string requestRequestText)
    {
        Guard.Against.NullOrWhiteSpace(requestRequestText, nameof(requestRequestText));

        if (requestRequestText.Contains("list=")) // Проверяем, является ли ссылка плейлистом
        {
            var playlistId = ExtractPlaylistIdFromUrl(requestRequestText);
            ThrowIf.NullOrWhiteSpace(playlistId, nameof(playlistId));

            using var httpClient = new HttpClient();
            var url = $"https://www.googleapis.com/youtube/v3/playlistItems?part=contentDetails&maxResults=50&playlistId={playlistId}&key={_options.ApiKey}";

            var response = await httpClient.GetStringAsync(url);
            using var jsonDoc = JsonDocument.Parse(response);

            var items = jsonDoc.RootElement.GetProperty("items");
            var trackUrls = new List<string>();

            foreach (var item in items.EnumerateArray())
            {
                var videoId = item.GetProperty("contentDetails").GetProperty("videoId").GetString();
                if (!string.IsNullOrWhiteSpace(videoId))
                {
                    trackUrls.Add($"https://www.youtube.com/watch?v={videoId}");
                }
            }

            return trackUrls.ToArray();
        }
        else // Если это одиночное видео
        {
            var videoId = ExtractVideoIdFromUrl(requestRequestText);
            ThrowIf.NullOrWhiteSpace(videoId, nameof(videoId));

            return [$"https://www.youtube.com/watch?v={videoId}"];
        }
    }

    /// <summary>
    /// Извлекает идентификатор плейлиста из ссылки.
    /// </summary>
    private string? ExtractPlaylistIdFromUrl(string url)
    {
        var uri = new Uri(url);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        return query["list"];
    }

    /// <summary>
    /// Извлекает идентификатор видео из ссылки.
    /// </summary>
    private string? ExtractVideoIdFromUrl(string url)
    {
        var uri = new Uri(url);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        return query["v"];
    }

    /// <summary>
    /// Парсит длительность в формате ISO 8601.
    /// </summary>
    private TimeSpan ParseIso8601Duration(string iso8601Duration)
    {
        return System.Xml.XmlConvert.ToTimeSpan(iso8601Duration);
    }
}