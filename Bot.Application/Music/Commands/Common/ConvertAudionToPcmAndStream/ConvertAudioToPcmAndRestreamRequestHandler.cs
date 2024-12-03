using Ardalis.GuardClauses;
using Bot.Application.Common.Interfaces;
using MediatR;

namespace Bot.Application.Music.Commands.Common.ConvertAudionToPcmAndStream;

/// <summary>
/// Обработчик запроса на рестрим данных в другой поток с конвертацией в PCM.
/// </summary>
public class ConvertAudioToPcmAndRestreamRequestHandler : IRequestHandler<ConvertAudioToPcmAndRestreamRequest>
{
    private readonly IPcmAudioConverter _pcmAudioConverter;

    /// <summary>
    /// Обработчик запроса на рестрим данных в другой поток с конвертацией в PCM.
    /// </summary>
    public ConvertAudioToPcmAndRestreamRequestHandler(IPcmAudioConverter pcmAudioConverter)
    {
        _pcmAudioConverter = pcmAudioConverter;
    }

    /// <summary>
    /// Рестрим данных в другой поток с конвертацией в PCM.
    /// </summary>
    public Task Handle(ConvertAudioToPcmAndRestreamRequest request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        return _pcmAudioConverter.ConvertAndStreamAsync(request.AudioStream, request.ReStreamAction, cancellationToken);
    }
}