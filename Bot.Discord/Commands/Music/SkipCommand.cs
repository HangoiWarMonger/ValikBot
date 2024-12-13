using Bot.Application.Music.Commands.Common.SkipTrack;
using Bot.Discord.Common.Graphics.Embed;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MediatR;

namespace Bot.Discord.Commands.Music;

[ModuleLifespan(ModuleLifespan.Transient)]
public class SkipCommand : BaseCommandModule
{
    private readonly ISender _sender;

    public SkipCommand(ISender sender)
    {
        _sender = sender;
    }

    [Command("skip")]
    public async Task Skip(CommandContext ctx)
    {
        var voiceChannel = GetVoiceChannel(ctx.Member);

        if (voiceChannel is null)
        {
            await ctx.Channel.SendMessageAsync($"В канал зайди идиот");
            return;
        }

        await ctx.Channel.SendMessageAsync(MusicEmbed.TrackSkip(ctx.Member!));

        var skip = new SkipTrackRequest(ctx.Guild.Id);
        await _sender.Send(skip);
    }

    private DiscordChannel? GetVoiceChannel(DiscordMember? member)
    {
        return member?.VoiceState?.Channel;
    }
}