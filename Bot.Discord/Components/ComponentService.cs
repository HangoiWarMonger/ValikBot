using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace Bot.Discord.Components;

public class ComponentService
{
    private readonly List<InteractiveComponent> _interactions;

    public ComponentService()
    {
        _interactions = [];
    }

    public void Register(DiscordComponent discordComponent, Func<DiscordClient, ComponentInteractionCreateEventArgs, Task> action)
    {
        var interactiveComponent = new InteractiveComponent(discordComponent.CustomId, action);
        _interactions.Add(interactiveComponent);
    }

    public async Task ExecuteAsync(DiscordClient client, ComponentInteractionCreateEventArgs args)
    {
        var component = _interactions.FirstOrDefault(x => x.Id == args.Id);

        if (component is null) return;

        await component.Action(client, args);
    }
}