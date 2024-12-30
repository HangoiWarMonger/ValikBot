using System.Text.Json;
using Bot.Application.Common.Dto;
using Bot.Application.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace Bot.Infrastructure.SoundCloud;

public class SoundCloudTrackClient : ITrackClient
{
    private const string ApiBaseUrl = "https://api-widget.soundcloud.com";

    private readonly HttpClient _httpClient;
    private readonly string _clientId;

    public SoundCloudTrackClient(HttpClient httpClient, IOptions<SoundCloudOptions> options)
    {
        _httpClient = httpClient;
        _clientId = options.Value.ClientId;
    }

    public SoundCloudTrackClient(HttpClient httpClient, string clientId)
    {
        _httpClient = httpClient;
        _clientId = clientId;
    }

    private async Task<JsonDocument> GetAllTrackInfo(string url, CancellationToken cancellationToken = default)
    {
        var infoUrl = $"{ApiBaseUrl}/resolve?url={url}&format=json&client_id={_clientId}";
        var response = await _httpClient.GetStringAsync(infoUrl, cancellationToken);

        return JsonDocument.Parse(response);
    }

    public async Task<Stream> GetAudioStreamAsync(string videoUrl, CancellationToken cancellationToken = default)
    {
        var json = await GetAllTrackInfo(videoUrl, cancellationToken);

        var transcodings = json.RootElement
            .GetProperty("media")
            .GetProperty("transcodings")
            .EnumerateArray();

        var streamTranscoding = transcodings.FirstOrDefault(t =>
            t.GetProperty("format").GetProperty("protocol").GetString() == "hls");

        if (streamTranscoding.ValueKind == JsonValueKind.Undefined)
            throw new InvalidOperationException("No hls stream found for the given track.");

        var streamUrl = streamTranscoding.GetProperty("url").GetString();
        var trackAuthorization = json.RootElement.GetProperty("track_authorization").GetString();

        if (string.IsNullOrEmpty(streamUrl) || string.IsNullOrEmpty(trackAuthorization))
            throw new InvalidOperationException("Stream URL or track authorization not found.");

        var preAudioStreamUrl = $"{streamUrl}?client_id={_clientId}&track_authorization={trackAuthorization}";
        var audioStreamUrlResponse = await _httpClient.GetStringAsync(preAudioStreamUrl, cancellationToken);
        json = JsonDocument.Parse(audioStreamUrlResponse);

        var audioStreamUrl = json.RootElement.GetProperty("url").GetString();

        // Создаем запрос с учетом заголовков
        var request = new HttpRequestMessage(HttpMethod.Get, audioStreamUrl);
        request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.5672.63 Safari/537.36");
        request.Headers.Referrer = new Uri(videoUrl);

        // Выполняем запрос на поток
        var audioStreamResponse = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        if (!audioStreamResponse.IsSuccessStatusCode)
            throw new HttpRequestException($"Failed to get audio stream. Status code: {audioStreamResponse.StatusCode}");

        // Возвращаем поток
        var sourceStream = await audioStreamResponse.Content.ReadAsStreamAsync(cancellationToken);
        var memoryStream = new MemoryStream();
        await sourceStream.CopyToAsync(memoryStream, cancellationToken);

        // Устанавливаем позицию на начало для последующего чтения
        memoryStream.Position = 0;

        return memoryStream;
    }

    public async Task<TrackInfoDto> GetInfoAsync(string url)
    {
        var json = await GetAllTrackInfo(url);

        return new TrackInfoDto
        {
            Title = json.RootElement.GetProperty("title").GetString()!,
            Duration = TimeSpan.FromMilliseconds(json.RootElement.GetProperty("duration").GetInt32()),
            Url = url,
            ThumbnailUrl = json.RootElement.GetProperty("artwork_url").GetString()!,
        };
    }

    public async Task<string[]> GetTracksFromLink(string requestRequestText)
    {
        var json = await GetAllTrackInfo(requestRequestText);

        // Определяем тип ресурса
        var kind = json.RootElement.GetProperty("kind").GetString();

        if (kind == "track")
        {
            // Если это одиночный трек, возвращаем массив с одной ссылкой
            return [requestRequestText];
        }
        else if (kind == "playlist")
        {
            // Если это плейлист, извлекаем ссылки на все треки
            var tracks = json.RootElement.GetProperty("tracks").EnumerateArray();
            var trackIds = tracks
                .Select(track => track.GetProperty("id").GetInt32())
                .ToArray();

            var trackUrls = new List<string>();

            var trackUrlTasks = trackIds.Select(trackId => _httpClient.GetStringAsync($"https://api-v2.soundcloud.com/tracks/{trackId}?client_id={_clientId}")).ToList();

            foreach (var trackUrlTask in trackUrlTasks)
            {
                json = JsonDocument.Parse(await trackUrlTask);
                var trackUrl = json.RootElement.GetProperty("permalink_url").GetString();

                trackUrls.Add(trackUrl!);
            }

            return trackUrls.ToArray();
        }

        // Если формат неизвестен, бросаем исключение
        throw new InvalidOperationException("Unsupported SoundCloud resource type.");
    }
}