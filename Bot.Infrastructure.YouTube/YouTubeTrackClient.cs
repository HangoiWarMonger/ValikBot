using System.Diagnostics;
using System.Text.Json;
using Ardalis.GuardClauses;
using Bot.Application.Common.Dto;
using Bot.Application.Common.Interfaces;
using Bot.Domain.Validation;
using Microsoft.Extensions.Options;

namespace Bot.Infrastructure.YouTube;

/// <summary>
/// Контракт работы клиента, работающего с треками.
/// </summary>
public class YouTubeTrackClient : ITrackClient
{
    private const string SearchUrl = "https://www.googleapis.com/youtube/v3/search";
    private const string VideoInfoUrl = "https://www.googleapis.com/youtube/v3/videos";

    private readonly YoutubeApiOptions _options;

    public YouTubeTrackClient(IOptions<YoutubeApiOptions> options)
    {
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

        //string ytDlpPath = @"D:\yt-dlp.exe";
       //ytDlpPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "yt-dlp.exe");
        var cookiesPath = "cookies.txt";

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = _options.YtDlpPath,
            Arguments = $"-f bestaudio --no-post-overwrites --cookies {cookiesPath} --quiet -o - {videoUrl}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = Process.Start(startInfo);
        ThrowIf.Null(process, nameof(process));

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

    private string? ExtractVideoIdFromUrl(string url)
    {
        var uri = new Uri(url);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        return query["v"];
    }

    private TimeSpan ParseIso8601Duration(string iso8601Duration)
    {
        return System.Xml.XmlConvert.ToTimeSpan(iso8601Duration);
    }
}