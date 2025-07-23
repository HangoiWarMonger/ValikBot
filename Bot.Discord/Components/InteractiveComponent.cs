using DSharpPlus;
using DSharpPlus.EventArgs;

namespace Bot.Discord.Components;

/// <summary>
/// Интерактивный UI‑компонент Discord.
/// </summary>
public class InteractiveComponent
{
    /// <summary>
    /// Идентификатор компонента.
    /// </summary>
    public string Id { get; init; }

    /// <summary>
    /// Действие при активации компонента.
    /// </summary>
    public Func<DiscordClient, ComponentInteractionCreateEventArgs, Task> Action { get; init; }

    /// <summary>
    /// Создаёт интерактивный компонент.
    /// </summary>
    public InteractiveComponent(string id, Func<DiscordClient, ComponentInteractionCreateEventArgs, Task> action)
    {
        Id = id;
        Action = action;
    }
}