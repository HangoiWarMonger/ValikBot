using MediatR;

namespace Bot.Application.Music.Commands.Common.GetAudioStream;

/// <summary>
/// Запрос на получение аудио потока из ютуб сслыки.
/// </summary>
public class GetAudioStreamRequest : IRequest<Stream>
{
    /// <summary>
    /// Ссылка на ютуб видео.
    /// </summary>
    public string Url { get; init; }

    /// <summary>
    /// Запрос на получение аудио потока из ютуб сслыки.
    /// </summary>
    public GetAudioStreamRequest(string url)
    {
        Url = url;
    }
}