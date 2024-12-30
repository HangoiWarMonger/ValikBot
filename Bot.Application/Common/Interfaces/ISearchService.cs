namespace Bot.Application.Common.Interfaces;

/// <summary>
/// Контракт сервиса для поиска трека по названию.
/// </summary>
public interface ISearchService
{
    /// <summary>
    /// Поиск ссылки на трек по запросу.
    /// </summary>
    /// <param name="request">Запрос.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Ссылка на трек.</returns>
    Task<string?> SearchTrackUrlAsync(string request, CancellationToken cancellationToken = default);
}