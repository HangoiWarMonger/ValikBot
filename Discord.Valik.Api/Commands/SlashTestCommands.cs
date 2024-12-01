using Discord.Valik.Api.Commands.Music;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.VoiceNext;
using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Pipes;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace Discord.Valik.Api.Commands;

public class SlashTestCommands : ApplicationCommandModule
{
    private YoutubeClient _youtubeClient;
    
    [SlashCommand("play", "Slash test command")]
    public async Task SlashTestCommand(InteractionContext ctx, [Option("url", "yt url")] string url)
    {
        await ctx.DeferAsync();

        var channel = GetVoiceChannel(ctx.Member);
        
        if (channel is null)
        {
            var embedBuilder = new DiscordEmbedBuilder()
            {
                Title = "Ошибка!",
                Color = DiscordColor.Red,
                Description = "Войди в канал дурень!!"
            };
            
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embedBuilder));
            
            return;
        }
        
        var trackQueue = TrackQueuePool.GetTrackQueue(ctx.Guild.Id);
        trackQueue.Enqueue(url);
        
        var embBuilder = new DiscordEmbedBuilder
        {
            Title = "Добавляем в очередь",
            Color = DiscordColor.Green
        };
            
        await ctx.EditResponseAsync(new DiscordWebhookBuilder()
            .AddEmbed(embBuilder));
        
        if (!trackQueue.IsPlaying)
        {
            await PlayNextInQueue(trackQueue, channel, ctx.Client.GetVoiceNext());
        }
    }
    
    [SlashCommand("queue", "Текущая очередь воспроизведения")]
    public async Task GetQueue(InteractionContext ctx)
    {
        await ctx.DeferAsync();

        var trackQueue = TrackQueuePool.GetTrackQueue(ctx.Guild.Id);
        if (!trackQueue.Any())
        {
            var embBuilder = new DiscordEmbedBuilder()
            {
                Title = "Ошибка!",
                Color = DiscordColor.Red,
                Description = "Нет треков!!"
            };
            
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .AddEmbed(embBuilder));
            return;
        }

        await ctx.Channel.SendMessageAsync("Треки");
        var index = 1;
        foreach (var item in trackQueue.GetAll())
        {
            await ctx.Channel.SendMessageAsync($"{index++} - {item}");
        }
    }

    [SlashCommand("skip", "Скипает текущий трек")]
    public async Task Skip(InteractionContext ctx)
    {
        await ctx.DeferAsync();

        var trackQueue = TrackQueuePool.GetTrackQueue(ctx.Guild.Id);

        if (!trackQueue.Any())
        {
            var embBuilder = new DiscordEmbedBuilder()
            {
                Title = "Ошибка!",
                Color = DiscordColor.Red,
                Description = "Нет треков!!"
            };
            
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .AddEmbed(embBuilder));
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