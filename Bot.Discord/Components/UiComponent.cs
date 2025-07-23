using DSharpPlus;
using DSharpPlus.Entities;

namespace Bot.Discord.Components;

/// <summary>
/// Готовые UI‑компоненты для взаимодействия с пользователем.
/// </summary>
public static class UiComponent
{
    /// <summary>
    /// Кнопка пропуска текущего трека.
    /// </summary>
    public static DiscordComponent SkipButton =
        new DiscordButtonComponent(ButtonStyle.Secondary, "skip_track", "Пропустить ->");
}