using Discord.Valik.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection()
    .BuildServiceProvider();

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var client = new BotClient(services, new BotSettings
{
    Token = configuration["BotConfig:Token"],
    Prefix = configuration["BotConfig:Prefix"],
});

//var stream = await PlayMusicYouTubeCommand.DownloadAudioFromUrl("https://www.youtube.com/watch?v=gxeuQoalPtw");
//var memoryStream = new MemoryStream();
//await PlayMusicYouTubeCommand.StreamAudioFileFFMPEG(stream, memoryStream);

await client.StartAsync();