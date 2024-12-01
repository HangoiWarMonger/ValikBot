using Discord.Valik.Api.Commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Builders;
using DSharpPlus.SlashCommands;
using DSharpPlus.VoiceNext;

namespace Discord.Valik.Api;

public class BotClient
{
    private readonly DiscordClient _discordClient;
    private readonly CommandsNextExtension _commandsNext;
    private readonly BotSettings _settings;

    public BotClient(BotSettings settings)
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
            EnableDefaultHelp = false
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