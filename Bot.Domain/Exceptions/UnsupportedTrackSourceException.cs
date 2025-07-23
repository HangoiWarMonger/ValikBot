namespace Bot.Domain.Exceptions;

/// <summary>
/// Исключение для неподдерживаемого источника треков.
/// </summary>
public class UnsupportedTrackSourceException : Exception
{
    /// <summary>
    /// Создаёт исключение с сообщением о неверной ссылке.
    /// </summary>
    public UnsupportedTrackSourceException() : base(ExceptionMessage.InvalidUrl())
    {
    }
}