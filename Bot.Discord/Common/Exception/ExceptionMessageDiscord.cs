using System.ComponentModel.DataAnnotations;
using Bot.Domain.Exceptions;

namespace Bot.Discord.Common.Exception;

public static class ExceptionMessageDiscord
{
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