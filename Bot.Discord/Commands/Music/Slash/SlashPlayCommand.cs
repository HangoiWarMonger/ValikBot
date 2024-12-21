using Bot.Application.Common.Dto;
using Bot.Application.Music.Commands.Common.EnqueueTrack;
using Bot.Application.Music.Commands.Common.GetTrackInfo;
using Bot.Application.Music.Commands.Common.GetUrlFromTextRequest;
using Bot.Application.Music.Commands.Common.PlayTrack;
using Bot.Discord.Common.Graphics.Embed;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.VoiceNext;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bot.Discord.Commands.Music.Slash;

/// <summary>
/// Команда добавления трека в очередь и воспроизведения его (если он первый в очереди).
/// </summary>
[ModuleLifespan(ModuleLifespan.Transient)]
public class SlashPlayCommand : ApplicationCommandModule
{
    private readonly ISender _sender;
    private readonly ILogger<SlashPlayCommand> _logger;

    /// <summary>
    /// Команда добавления трека в очередь и воспроизведения его (если он первый в очереди).
    /// </summary>
    public SlashPlayCommand(ISender sender, ILogger<SlashPlayCommand> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    /// <summary>
    /// Команда добавления трека в очередь и воспроизведения его (если он первый в очереди).
    /// </summary>
    [SlashCommand("play", "Добавить трек в очередь")]
    public async Task AddInQueue(InteractionContext ctx, [Option("запрос", "ссылка на ютуб видос или прост название")] string request)
    {
        await ctx.DeferAsync();

        _logger.LogDebug("Получение голосового канала к каналу.");
        var voiceChannel = GetVoiceChannel(ctx.Member);

        if (voiceChannel is null)
        {
            _logger.LogDebug("Пользователь не подключен к каналу.");
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(Embed.UserNotInVoiceChannelError(ctx.Member!)));
            return;
        }

        _logger.LogDebug("Получение url из запроса.");
        var getUrlRequest = new GetUrlFromTextRequest(request);
        var url = await _sender.Send(getUrlRequest);
        _logger.LogDebug("Url получен - {Url}", url);

        _logger.LogDebug("Получение информации о видео.");
        var getTrackInfoRequest = new GetTrackInfoRequest(url);
        TrackInfoDto trackInfo = await _sender.Send(getTrackInfoRequest);
        _logger.LogDebug("Информация получена.");

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(Embed.TrackQueued(trackInfo, ctx.Member!)));

        var enqueueTrack = new EnqueueTrackRequest(url, ctx.Guild.Id);
        await _sender.Send(enqueueTrack);

        _logger.LogDebug("Получение соединения с голосовым каналом");
        var voice = ctx.Client.GetVoiceNext();
        var connection = voice.GetConnection(voiceChannel.Guild) ?? await voice.ConnectAsync(voiceChannel);

        var transmit = connection.GetTransmitSink();

        _logger.LogDebug("Отправка запроса на проигрывание трека");
        await _sender.Send(
            new PlayTrackRequest(
                guildId: ctx.Guild.Id,
                restreamAction: (stream, token) => stream.CopyToAsync(transmit, cancellationToken: token),
                endStreamAction: transmit.FlushAsync()));
    }

    private DiscordChannel? GetVoiceChannel(DiscordMember? member)
    {
        return member?.VoiceState?.Channel;
    }
}