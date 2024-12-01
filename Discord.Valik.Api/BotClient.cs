using Discord.Valik.Api.Commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;
using DSharpPlus.VoiceNext;
using Microsoft.Extensions.DependencyInjection;

namespace Discord.Valik.Api;

public class BotClient
{
    private readonly DiscordClient _discordClient;
    private readonly CommandsNextExtension _commandsNext;
    private readonly BotSettings _settings;

    public BotClient(ServiceProvider services, BotSettings settings)
    {
        _settings = settings;

        var config = new DiscordConfiguration
        {
            Token = _settings.Token,
            TokenType = TokenType.Bot,
            AutoReconnect = true,
            Intents = DiscordIntents.All
        };

        _discordClient = new DiscordClient(config);

        var commandsConfig = new CommandsNextConfiguration
        {
            StringPrefixes = [_settings.Prefix],
            EnableMentionPrefix = true,
            EnableDms = false,
            EnableDefaultHelp = false,
            Services = services
        };

        var slashConfig = _discordClient.UseSlashCommands();
        slashConfig.RegisterCommands<SlashTestCommands>();

        _discordClient.UseVoiceNext(new VoiceNextConfiguration
        {
            EnableIncoming = false
        });

        _commandsNext = _discordClient.UseCommandsNext(commandsConfig);
        _commandsNext.RegisterCommands(typeof(Program).Assembly);
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await _discordClient.ConnectAsync();

        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(60), cancellationToken);
        }
    }
}