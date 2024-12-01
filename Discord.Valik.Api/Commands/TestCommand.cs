using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
namespace Discord.Valik.Api.Commands;

public class TestCommand : BaseCommandModule
{
    [Command("test")]
    public async Task Test(CommandContext ctx)
    {
        await ctx.Channel.SendMessageAsync("Testing command");
    }
}