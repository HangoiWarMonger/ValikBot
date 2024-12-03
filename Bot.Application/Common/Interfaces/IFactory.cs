namespace Bot.Application.Common.Interfaces;

/// <summary>
/// Контракт получения сервиса по заданному ключу.
/// </summary>
/// <typeparam name="TValue">Сервис.</typeparam>
/// <typeparam name="TKey">Ключ.</typeparam>
public interface IFactory<out TValue, in TKey>
{
    /// <summary>
    /// Получение сервиса по ключу.
    /// </summary>
    /// <param name="key">Ключ.</param>
    /// <returns>Сервис.</returns>
    TValue Get(TKey key);
}