namespace Bot.Domain.Exceptions;

/// <summary>
/// Сообщения об ошибках доменного уровня.
/// </summary>
public static class ExceptionMessage
{
    /// <summary>
    /// Сообщение о некорректной ссылке.
    /// </summary>
    public static string InvalidUrl()
    {
        return "Неподходящая ссылка.";
    }
}