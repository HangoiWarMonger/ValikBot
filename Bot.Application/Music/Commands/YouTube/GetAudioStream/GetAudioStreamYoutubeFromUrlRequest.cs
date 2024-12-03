using Bot.Application.Music.Commands.Common.GetAudioStream;

namespace Bot.Application.Music.Commands.YouTube.GetAudioStream;

/// <summary>
/// Запрос на получение аудио потока из ютуб сслыки.
/// </summary>
public class GetAudioStreamYoutubeFromUrlRequest : GetAudioStreamRequest
{
    public GetAudioStreamYoutubeFromUrlRequest(string url) : base(url)
    {
    }
}