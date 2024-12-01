using Discord.Valik.Api.Commands.Music;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Pipes;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace Discord.Valik.Api.Commands;

public class PlayMusicYouTubeCommand : BaseCommandModule
{
    private YoutubeClient _youtubeClient;

    [Command("play")]
    public async Task AddInQueue(CommandContext ctx, [RemainingText] string playUrl)
    {
        var channel = GetVoiceChannel(ctx.Member);

        if (channel is null)
        {
            await ctx.Channel.SendMessageAsync($"В канал зайд иидиот");
            return;
        }
        
        var trackQueue = TrackQueuePool.GetTrackQueue(ctx.Guild.Id);
        trackQueue.Enqueue(playUrl);
        
        await ctx.Channel.SendMessageAsync("Добавляем в очередь!");
        
        if (!trackQueue.IsPlaying)
        {
            await PlayNextInQueue(trackQueue, channel, ctx.Client.GetVoiceNext());
        }
    }

    [Command("queue")]
    public async Task GetQueue(CommandContext ctx)
    {
        var trackQueue = TrackQueuePool.GetTrackQueue(ctx.Guild.Id);
        if (!trackQueue.Any())
        {
            await ctx.Channel.SendMessageAsync("Нет треков!");
            return;
        }

        await ctx.Channel.SendMessageAsync("Треки");
        var index = 1;
        foreach (var item in trackQueue.GetAll())
        {
            await ctx.Channel.SendMessageAsync($"{index++} - {item}");
        }
    }

    [Command("skip")]
    public async Task Skip(CommandContext ctx)
    {
        var trackQueue = TrackQueuePool.GetTrackQueue(ctx.Guild.Id);

        if (!trackQueue.Any())
        {
            await ctx.Channel.SendMessageAsync($"Нет треков");
            return;
        }
        
        await trackQueue.SkipAsync();
    }

    private DiscordChannel? GetVoiceChannel(DiscordMember? member)
    {
        return member?.VoiceState?.Channel;
    }
    
    private async Task PlayNextInQueue(TrackQueue trackQueue, DiscordChannel voiceChannel, VoiceNextExtension voice)
    {
        if (trackQueue.TryDequeue(out var url))
        {
            trackQueue.IsPlaying = true;
            
            var connection = voice.GetConnection(voiceChannel.Guild) ?? await voice.ConnectAsync(voiceChannel);

            var transmit = connection.GetTransmitSink();

            try
            {
                _youtubeClient = new YoutubeClient();
                await using var audio = await GetAudioStreamAsync(url!, trackQueue.CancellationToken);
                await StreamAudioFileAsync(audio, transmit, trackQueue.CancellationToken);
            }
            finally
            {
                trackQueue.IsPlaying = false;
                await transmit.FlushAsync();
                
                await PlayNextInQueue(trackQueue, voiceChannel, voice);
            }
        }
    }
    
    private async Task<Stream> GetAudioStreamAsync(string url, CancellationToken cancellationToken = default)
    {
        var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(url, cancellationToken);
        var audioStreamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
        
        var audioStream = await _youtubeClient.Videos.Streams.GetAsync(audioStreamInfo, cancellationToken);
        
        return audioStream;
    }
    
    private async Task StreamAudioFileAsync(Stream audioStream, VoiceTransmitSink transmitSink, CancellationToken cancellationToken = default)
    {
        await FFMpegArguments.FromPipeInput(new StreamPipeSource(audioStream))
            .OutputToPipe(new StreamPipeSink((stream, _ ) => stream.CopyToAsync(transmitSink, cancellationToken: cancellationToken)), options => options
                .WithAudioBitrate(AudioQuality.Normal)
                .WithCustomArgument("-ar 48000")
                .ForceFormat("wav"))
            .ProcessAsynchronously(true, new FFOptions
            {
                BinaryFolder = "D:\\FFmpeg\\bin"
            });
    }
}