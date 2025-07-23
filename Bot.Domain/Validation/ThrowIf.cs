namespace Bot.Domain.Validation;

/// <summary>
/// Набор вспомогательных методов для проверки аргументов.
/// </summary>
public static class ThrowIf
{
    /// <summary>
    /// Бросает исключение, если строка пустая или null.
    /// </summary>
    public static void NullOrWhiteSpace(string? argument, string argumentName)
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            throw new ArgumentNullException($"String {argumentName} cannot be null or empty.");
        }
    }

    /// <summary>
    /// Бросает исключение, если объект равен null.
    /// </summary>
    public static void Null<T>(T? argument, string argumentName)
    {
        if (argument == null)
        {
            throw new ArgumentNullException($"Argument {argumentName} cannot be null.");
        }
    }
}