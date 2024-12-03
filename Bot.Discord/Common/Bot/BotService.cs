using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;
using DSharpPlus.VoiceNext;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Bot.Discord.Common.Bot;

public sealed class BotService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly BotSettings _settings;
    private DiscordClient _client;

    public BotService(IServiceProvider serviceProvider, IOptions<BotSettings> settings)
    {
        _serviceProvider = serviceProvider;
        _settings = settings.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var config = new DiscordConfiguration
        {
            Token = _settings.Token,
            TokenType = TokenType.Bot,
            AutoReconnect = true,
            Intents = DiscordIntents.All
        };

        _client = new DiscordClient(config);

        var commandsConfig = new CommandsNextConfiguration
        {
            StringPrefixes = [_settings.Prefix],
            EnableMentionPrefix = true,
            EnableDms = false,
            EnableDefaultHelp = false,
            Services = _serviceProvider
        };

        var slashConfig = _client.UseSlashCommands();
        //slashConfig.RegisterCommands<SlashTestCommands>();

        _client.UseVoiceNext(new VoiceNextConfiguration
        {
            EnableIncoming = false
        });

        var commandsNext = _client.UseCommandsNext(commandsConfig);
        commandsNext.RegisterCommands(typeof(Program).Assembly);

        await _client.ConnectAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _client.DisconnectAsync();
        _client.Dispose();
    }
}