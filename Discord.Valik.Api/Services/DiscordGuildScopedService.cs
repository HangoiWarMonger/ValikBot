using System.Collections.Concurrent;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Discord.Valik.Api.Services;

public class DiscordGuildScopedService<T> : IDiscordGuildScopedService<T> where T : class
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentDictionary<ulong, T> _services = new();

    public DiscordGuildScopedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public T GetServiceForGuild(DiscordGuild guild)
    {
        return _services.GetOrAdd(guild.Id, _ => CreateService());
    }

    private T CreateService()
    {
        var scope = _serviceProvider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<T>();

        return service;
    }
}