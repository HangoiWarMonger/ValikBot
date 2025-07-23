using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace Bot.Discord.Components;

/// <summary>
/// Сервис для работы с интерактивными компонентами.
/// </summary>
public class ComponentService
{
    private readonly List<InteractiveComponent> _interactions;

    /// <summary>
    /// Создаёт экземпляр сервиса.
    /// </summary>
    public ComponentService()
    {
        _interactions = [];
    }

    /// <summary>
    /// Регистрирует новый интерактивный компонент.
    /// </summary>
    public void Register(DiscordComponent discordComponent, Func<DiscordClient, ComponentInteractionCreateEventArgs, Task> action)
    {
        var interactiveComponent = new InteractiveComponent(discordComponent.CustomId, action);
        _interactions.Add(interactiveComponent);
    }

    /// <summary>
    /// Выполняет действие для пришедшего взаимодействия.
    /// </summary>
    public async Task ExecuteAsync(DiscordClient client, ComponentInteractionCreateEventArgs args)
    {
        var component = _interactions.FirstOrDefault(x => x.Id == args.Id);

        if (component is null) return;

        await component.Action(client, args);
    }
}