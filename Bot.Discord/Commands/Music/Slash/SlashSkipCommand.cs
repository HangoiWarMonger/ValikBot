using Bot.Application.Music.Commands.Common.SkipTrack;
using Bot.Application.Music.Commands.Common.TrackQueueIsEmpty;
using Bot.Discord.Common.Graphics.Embed;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bot.Discord.Commands.Music.Slash;

/// <summary>
/// Команда для пропуска текущего трека.
/// </summary>
[ModuleLifespan(ModuleLifespan.Transient)]
public class SlashSkipCommand : ApplicationCommandModule
{
    private readonly ISender _sender;
    private readonly ILogger<SlashSkipCommand> _logger;

    /// <summary>
    /// Команда для пропуска текущего трека.
    /// </summary>
    public SlashSkipCommand(ISender sender, ILogger<SlashSkipCommand> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    /// <summary>
    /// Команда для пропуска текущего трека.
    /// </summary>
    [SlashCommand("skip", "Пропустить текущий трек")]
    public async Task Skip(InteractionContext ctx)
    {
        await ctx.DeferAsync();

        _logger.LogDebug("Получение голосового канала...");
        var voiceChannel = GetVoiceChannel(ctx.Member);

        if (voiceChannel is null)
        {
            _logger.LogDebug("Попытка выполнения команды без подключения к голосовому каналу.");
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(Embed.UserNotInVoiceChannelError(ctx.Member!)));

            return;
        }

        _logger.LogDebug("Получение данных, полная очередь или нет.");
        var queueIsEmpty = await _sender.Send(new TrackQueueIsEmptyRequest(ctx.Guild.Id));
        _logger.LogDebug("Очередь пустая - {QueueIsEmpty}.", queueIsEmpty);

        if (queueIsEmpty)
        {
            _logger.LogDebug("Очередь пустая.");
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(Embed.TrackQueueIsEmptyError(ctx.Member!)));

            return;
        }

        _logger.LogDebug("Пропускаем трек...");
        var skip = new SkipTrackRequest(ctx.Guild.Id);
        await _sender.Send(skip);
    }

    private DiscordChannel? GetVoiceChannel(DiscordMember? member)
    {
        return member?.VoiceState?.Channel;
    }
}