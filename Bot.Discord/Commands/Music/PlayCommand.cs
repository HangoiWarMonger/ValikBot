using Bot.Application.Common.Dto;
using Bot.Application.Music.Commands.Common.EnqueueTrack;
using Bot.Application.Music.Commands.Common.GetTrackInfo;
using Bot.Application.Music.Commands.Common.PlayTrack;
using Bot.Discord.Common.Graphics.Embed;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using MediatR;

namespace Bot.Discord.Commands.Music;

[ModuleLifespan(ModuleLifespan.Transient)]
public class PlayCommand : BaseCommandModule
{
    private readonly ISender _sender;

    public PlayCommand(ISender sender)
    {
        _sender = sender;
    }

    [Command("play")]
    public async Task AddInQueue(CommandContext ctx, [RemainingText] string playUrl)
    {
        var voiceChannel = GetVoiceChannel(ctx.Member);

        if (voiceChannel is null)
        {
            await ctx.Channel.SendMessageAsync($"В канал зайд иидиот");
            return;
        }

        var getTrackInfoRequest = new GetTrackInfoRequest(playUrl);
        TrackInfoDto trackInfo = await _sender.Send(getTrackInfoRequest);

        await ctx.Channel.SendMessageAsync(MusicEmbed.TrackEmbed(trackInfo, ctx.Member!));

        var enqueueTrack = new EnqueueTrackRequest(playUrl, ctx.Guild.Id);
        await _sender.Send(enqueueTrack);

        var voice = ctx.Client.GetVoiceNext();
        var connection = voice.GetConnection(voiceChannel.Guild) ?? await voice.ConnectAsync(voiceChannel);

        var transmit = connection.GetTransmitSink();

        await _sender.Send(
            new PlayTrackRequest(
                guildId: ctx.Guild.Id,
                restreamAction: (stream) => stream.CopyToAsync(transmit),
                endStreamAction: transmit.FlushAsync()));
    }

    private DiscordChannel? GetVoiceChannel(DiscordMember? member)
    {
        return member?.VoiceState?.Channel;
    }
}