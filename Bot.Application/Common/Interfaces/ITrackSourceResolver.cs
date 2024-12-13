using Bot.Application.Common.Types;

namespace Bot.Application.Common.Interfaces;

/// <summary>
/// Контракт сервиса, определяющего источник ссылки.
/// </summary>
public interface ITrackSourceResolver
{
    /// <summary>
    /// Определение источника ссылки.
    /// </summary>
    /// <param name="url">Ссылка.</param>
    TrackSource GetTrackSource(string url);
}