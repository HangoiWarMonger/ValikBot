namespace Bot.Application.Common.Interfaces;

public interface IPcmAudioConverter
{
    /// <summary>
    /// Переконвертирует поток данных в pcm формат в поток (метод записи задается в делегате).
    /// </summary>
    /// <param name="audioStream">Аудио поток.</param>
    /// <param name="reStreamAction">Метод записи сконвертированного потока.</param>
    /// <param name="cancellationToken">Токен для отмены операции.</param>
    Task ConvertAndStreamAsync(Stream audioStream, Func<Stream, Task> reStreamAction, CancellationToken cancellationToken = default);
}