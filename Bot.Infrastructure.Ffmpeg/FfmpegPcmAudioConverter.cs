using Ardalis.GuardClauses;
using Bot.Application.Common.Interfaces;
using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Pipes;
using Microsoft.Extensions.Options;

namespace Bot.Infrastructure.Ffmpeg;

/// <summary>
/// FFMPEG ковертер аудио в PCM формат.
/// </summary>
public class FfmpegPcmAudioConverter : IPcmAudioConverter
{
    private readonly FfmpegOptions _options;

    public FfmpegPcmAudioConverter(IOptions<FfmpegOptions> options)
    {
        _options = Guard.Against.Null(options.Value, nameof(options));
    }

    /// <summary>
    /// Переконвертирует поток данных в pcm формат в поток (метод записи задается в делегате).
    /// </summary>
    /// <param name="audioStream">Аудио поток.</param>
    /// <param name="reStreamAction">Метод записи сконвертированного потока.</param>
    /// <param name="cancellationToken">Токен для отмены операции.</param>
    public async Task ConvertAndStreamAsync(Stream audioStream, Func<Stream, CancellationToken, Task> reStreamAction, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(audioStream);
        Guard.Against.Null(reStreamAction);

        await FFMpegArguments.FromPipeInput(new StreamPipeSource(audioStream))
            .OutputToPipe(new StreamPipeSink((stream, _) => reStreamAction(stream, cancellationToken)), options => options
                .WithAudioBitrate(AudioQuality.Normal)
                .WithCustomArgument("-ar 48000")
                .ForceFormat("wav"))
            .ProcessAsynchronously(false, new FFOptions
            {
                BinaryFolder = _options.BinaryPath
            });
    }
}