using DSharpPlus;
using DSharpPlus.Entities;

namespace Bot.Discord.Components;

public static class UiComponent
{
    public static DiscordComponent SkipButton =
        new DiscordButtonComponent(ButtonStyle.Secondary, "skip_track", "Пропустить ->");
}