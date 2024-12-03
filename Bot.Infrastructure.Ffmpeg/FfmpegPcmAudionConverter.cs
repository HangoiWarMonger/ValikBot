using Ardalis.GuardClauses;
using Bot.Application.Common.Interfaces;
using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Pipes;

namespace Bot.Infrastructure.Ffmpeg;

public class FfmpegPcmAudionConverter : IPcmAudioConverter
{
    /// <summary>
    /// Переконвертирует поток данных в pcm формат в поток (метод записи задается в делегате).
    /// </summary>
    /// <param name="audioStream">Аудио поток.</param>
    /// <param name="reStreamAction">Метод записи сконвертированного потока.</param>
    /// <param name="cancellationToken">Токен для отмены операции.</param>
    public async Task ConvertAndStreamAsync(Stream audioStream, Func<Stream, Task> reStreamAction, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(audioStream, nameof(audioStream));
        Guard.Against.Null(reStreamAction, nameof(reStreamAction));

        await FFMpegArguments.FromPipeInput(new StreamPipeSource(audioStream))
            .OutputToPipe(new StreamPipeSink((stream, _) => reStreamAction(stream)), options => options
                .WithAudioBitrate(AudioQuality.Normal)
                .WithCustomArgument("-ar 48000")
                .ForceFormat("wav"))
            .ProcessAsynchronously(true, new FFOptions
            {
                BinaryFolder = "D:\\FFmpeg\\bin"
            });
    }
}