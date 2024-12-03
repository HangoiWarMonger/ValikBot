using DSharpPlus.Entities;

namespace Discord.Valik.Api.Services;

public interface IDiscordGuildScopedService<out T>
{
    T GetServiceForGuild(DiscordGuild guild);
}