using MediatR;

namespace Bot.Application.Music.Commands.Common.ConvertAudionToPcmAndStream;

/// <summary>
/// Запрос на рестрим данных в другой поток.
/// </summary>
public class ConvertAudioToPcmAndRestreamRequest : IRequest
{
    /// <summary>
    /// Входящий аудио поток.
    /// </summary>
    public Stream AudioStream { get; init; }

    /// <summary>
    /// Функция того, как и куда будет происходить перезапись потока данных.
    /// </summary>
    public Func<Stream, Task> ReStreamAction { get; init; }

    /// <summary>
    /// Запрос на рестрим данных в другой поток.
    /// </summary>
    public ConvertAudioToPcmAndRestreamRequest(Stream audioStream, Func<Stream, Task> reStreamAction)
    {
        AudioStream = audioStream;
        ReStreamAction = reStreamAction;
    }
}