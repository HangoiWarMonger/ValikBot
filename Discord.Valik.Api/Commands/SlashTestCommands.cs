using System.Diagnostics;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.VoiceNext;
using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Pipes;
using NAudio.Wave;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace Discord.Valik.Api.Commands;

public class SlashTestCommands : ApplicationCommandModule
{
    [SlashCommand("play", "Slash test command")]
    public async Task SlashTestCommand(InteractionContext ctx, [Option("url", "yt url")] string url)
    {
        var voiceChannel  = ctx.Member?.VoiceState?.Channel;
        
        if (voiceChannel == null)
        {
            await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, 
                new DiscordInteractionResponseBuilder()
                    .WithContent("Ты в канал готововой зайди, гений"));
            return;
        }

        var voice = ctx.Client.GetVoiceNext();

        VoiceNextConnection? connection = null;
        if (voice.GetConnection(ctx.Member?.Guild) != null)
        {
            connection = await voice.ConnectAsync(voiceChannel);    
        }
        
        if (connection == null) return;

        var transmit = connection.GetTransmitSink();
        
        await using var audio = await GetAudioStreamAsync(url);
        
        await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, 
            new DiscordInteractionResponseBuilder()
                .WithContent("Играю музыку..."));
        
        await StreamAudioFileAsync(audio, transmit);
        await transmit.FlushAsync();
        
      
    }
    
    private async Task<Stream> GetAudioStreamAsync(string url, CancellationToken cancellationToken = default)
    {
        var youTube = new YoutubeClient();
        //var title = (await youTube.Videos.GetAsync(url, cancellationToken)).Title;
        var streamManifest = await youTube.Videos.Streams.GetManifestAsync(url, cancellationToken);

        var audioStreamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
        
        var audioStream = await youTube.Videos.Streams.GetAsync(audioStreamInfo, cancellationToken);
        
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