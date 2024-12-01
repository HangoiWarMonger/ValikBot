using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.VoiceNext;
using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Pipes;
using NAudio.Wave;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace Discord.Valik.Api.Commands;

public class PlayMusicYouTubeCommand : BaseCommandModule
{
    private YoutubeClient _youtubeClient;
    
    [Command("slashtest")]
    public async Task SlashTestCommand(CommandContext ctx, [RemainingText] string url)
    {
        var voiceChannel  = ctx.Member?.VoiceState?.Channel;
        
        if (voiceChannel == null)
        {
            await ctx.Channel.SendMessageAsync("Ты в канал готововой зайди, гений");
            return;
        }

        var voice = ctx.Client.GetVoiceNext();

        var connection = await voice.ConnectAsync(voiceChannel);
        
        if (connection == null) return;

        var transmit = connection.GetTransmitSink();
        
        _youtubeClient = new YoutubeClient();
        
        await using var audio = await GetAudioStreamAsync(url);
        await StreamAudioFileAsync(audio, transmit);
        await transmit.FlushAsync();
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