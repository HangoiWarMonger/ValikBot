using Bot.Discord.Common;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Bot.Discord.Common.Bot;

namespace Bot.Discord.Commands;

[ModuleLifespan(ModuleLifespan.Transient)]
public class TestCommand(ITest test) : BaseCommandModule
{
    [Command("test")]
    public async Task Test(CommandContext ctx)
    {
        await ctx.Channel.SendMessageAsync("asf");
        //var token = serviceProvider.GetRequiredService<IConfiguration>()[$"{BotSettings.SectionName}:Prefix"];
        await ctx.Channel.SendMessageAsync($"Ичсло: {test.GetNumber()}");
    }
}