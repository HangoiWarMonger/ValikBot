using System.Collections.Concurrent;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.VoiceNext;
using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Pipes;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace Discord.Valik.Api.Commands;

public class QueuedTrack
{
    public string Url { get; init; }
}

public class PlayMusicYouTubeCommand : BaseCommandModule
{
    private YoutubeClient _youtubeClient;
    
    private static readonly ConcurrentDictionary<ulong, ConcurrentQueue<QueuedTrack>> _playQueue = new();
    
    private static readonly ConcurrentBag<ulong> _currentGuilds = [];
    private static readonly ConcurrentDictionary<ulong, bool> _currentPlayingGuilds = [];
    private static readonly ConcurrentDictionary<ulong, CancellationTokenSource> _cancellationTokenSources = new();

    [Command("play")]
    public async Task AddInQueue(CommandContext ctx, [RemainingText] string playUrl)
    {
        if (!_playQueue.ContainsKey(ctx.Guild.Id))
        {
            _playQueue.TryAdd(ctx.Guild.Id, new ConcurrentQueue<QueuedTrack>());
        }
        
        _playQueue[ctx.Guild.Id].Enqueue(new QueuedTrack
        {
            Url = playUrl
        });

        await ctx.Channel.SendMessageAsync("Добавляем в очередь!");

        if (!_currentPlayingGuilds.ContainsKey(ctx.Guild.Id) || !_currentPlayingGuilds[ctx.Guild.Id])
        {
            await ctx.Channel.SendMessageAsync("Нет треков в очереди");
            await PlayNextInQueue(ctx);
        }
        else
        {
            await ctx.Channel.SendMessageAsync("Есть треки в очереди не играем ((");
        }
    }

    [Command("queue")]
    public async Task Getqueue(CommandContext ctx)
    {
        if (!_playQueue.ContainsKey(ctx.Guild.Id))
        {
            await ctx.Channel.SendMessageAsync($"Нет треков!");
            return;
        }

        await ctx.Channel.SendMessageAsync($"Треки");
        var index = 1;
        foreach (var item in _playQueue[ctx.Guild.Id])
        {
            await ctx.Channel.SendMessageAsync($"{index++} - {item}");
        }
    }

    [Command("skip")]
    public async Task Skip(CommandContext ctx)
    {
        if (!_cancellationTokenSources.TryGetValue(ctx.Guild.Id, out var source))
        {
            await ctx.Channel.SendMessageAsync($"Нет треков");
            return;
        }
        
        await source.CancelAsync();
        _currentPlayingGuilds.TryRemove(ctx.Guild.Id, out _);
        _cancellationTokenSources.TryRemove(ctx.Guild.Id, out _);
    }
    
    private async Task PlayNextInQueue(CommandContext ctx)
    {
        if (_playQueue[ctx.Guild.Id].TryDequeue(out var queuedTrack))
        {
            _cancellationTokenSources.TryAdd(ctx.Guild.Id, new CancellationTokenSource());
            _currentPlayingGuilds.TryAdd(ctx.Guild.Id, true);
            var voiceChannel  = ctx.Member?.VoiceState?.Channel;
            
            if (voiceChannel == null)
            {
                await ctx.Channel.SendMessageAsync("Ты в канал готововой зайди, гений");
                return;
            }
            
            var voice = ctx.Client.GetVoiceNext();
        
            var guild = ctx.Guild;
            
            VoiceNextConnection connection;
            if (_currentGuilds.Contains(guild.Id))
            {
                connection = voice.GetConnection(guild);    
            }
            else
            {
                _currentGuilds.Add(guild.Id);
                connection = await voice.ConnectAsync(voiceChannel);
            }

            var cancellationToken = _cancellationTokenSources[ctx.Guild.Id].Token;
            var transmit = connection.GetTransmitSink();

            _youtubeClient = new YoutubeClient();

            try
            {
                await using var audio = await GetAudioStreamAsync(queuedTrack.Url, cancellationToken);
                await StreamAudioFileAsync(audio, transmit, cancellationToken);
            }
            finally
            {
                await transmit.FlushAsync();
                _currentPlayingGuilds.TryAdd(ctx.Guild.Id, false);
                
                await PlayNextInQueue(ctx);
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