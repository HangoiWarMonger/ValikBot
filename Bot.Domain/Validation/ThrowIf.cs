namespace Bot.Domain.Validation;

public static class ThrowIf
{
    public static void NullOrWhiteSpace(string? argument, string argumentName)
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            throw new ArgumentNullException($"String {argumentName} cannot be null or empty.");
        }
    }

    public static void Null<T>(T? argument, string argumentName)
    {
        if (argument == null)
        {
            throw new ArgumentNullException($"Argument {argumentName} cannot be null.");
        }
    }
}