using System.ComponentModel.DataAnnotations;
using Bot.Domain.Exceptions;

namespace Bot.Discord.Common.Exception;

/// <summary>
/// Помощник для преобразования исключений в человекочитаемые сообщения.
/// </summary>
public static class ExceptionMessageDiscord
{
    /// <summary>
    /// Возвращает понятное сообщение по исключению.
    /// </summary>
    public static string GetHumanMessage(System.Exception exception)
    {
        return exception switch
        {
            ValidationException validationException => validationException.Message,
            UnsupportedTrackSourceException => exception.Message,
            _ => "Неизвестная ошибка.."
        };
    }
}