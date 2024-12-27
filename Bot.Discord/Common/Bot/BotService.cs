using Bot.Discord.Commands.Music.Slash;
using Bot.Discord.Common.Graphics.Embed;
using Bot.Discord.Components;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.VoiceNext;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bot.Discord.Common.Bot;

public sealed class BotService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly BotSettings _settings;
    private readonly ILogger<BotService> _logger;
    private readonly ComponentService _componentService;
    private DiscordClient _client;

    public BotService(IServiceProvider serviceProvider, IOptions<BotSettings> settings, ILogger<BotService> logger, ComponentService componentService)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _componentService = componentService;
        _settings = settings.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Запускаем бота.");
        _logger.LogInformation("Токен бота - {DiscordToken}", _settings.Token);
        _logger.LogInformation("Префикс - {DiscordToken}", _settings.Prefix);

        var config = new DiscordConfiguration
        {
            Token = _settings.Token,
            TokenType = TokenType.Bot,
            AutoReconnect = true,
            Intents = DiscordIntents.All
        };

        _logger.LogInformation("Создаем клиент.");
        _client = new DiscordClient(config);

        var commandsConfig = new CommandsNextConfiguration
        {
            StringPrefixes = [_settings.Prefix],
            EnableMentionPrefix = true,
            EnableDms = false,
            EnableDefaultHelp = false,
            Services = _serviceProvider
        };
        var slashCommands = new SlashCommandsConfiguration
        {
            Services = _serviceProvider,
        };

        var slashConfig = _client.UseSlashCommands(slashCommands);
        slashConfig.RegisterCommands<SlashPlayCommand>();
        slashConfig.RegisterCommands<SlashSkipCommand>();

        slashConfig.SlashCommandErrored += async (_, args) =>
        {
            switch (args.Exception)
            {
                case ObjectDisposedException:
                case IOException {Message: "Broken pipe"}:
                    return;
            }

            _logger.LogCritical($"Exception: {args.Exception.GetType().Name}: {args.Exception.Message}");
            _logger.LogError("{Trace}", args.Exception.StackTrace);

            await args.Context.EditResponseAsync(new DiscordWebhookBuilder()
                .AddEmbed(Embed.Error(args.Context.Member, args.Exception.Message)));
        };

        _client.ComponentInteractionCreated += async (client, args) =>
        {
            await _componentService.ExecuteAsync(client, args);
        };

        _client.UseVoiceNext(new VoiceNextConfiguration
        {
            EnableIncoming = false
        });

        var commandsNext = _client.UseCommandsNext(commandsConfig);
        commandsNext.RegisterCommands(typeof(Program).Assembly);

        _logger.LogInformation("Подключение...");
        await _client.ConnectAsync();
        _logger.LogInformation("Успешное подключение...");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Завершение работы...");
        await _client.DisconnectAsync();
        _client.Dispose();
        _logger.LogInformation("Бот выключен.");
    }
}