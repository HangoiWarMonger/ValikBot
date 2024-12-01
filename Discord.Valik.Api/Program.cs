// See https://aka.ms/new-console-template for more information

using Discord.Valik.Api;
using Discord.Valik.Api.Commands;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var client = new BotClient(new BotSettings
{
    Token = configuration["BotConfig:Token"],
    Prefix = configuration["BotConfig:Prefix"],
});

//var stream = await PlayMusicYouTubeCommand.DownloadAudioFromUrl("https://www.youtube.com/watch?v=gxeuQoalPtw");
//var memoryStream = new MemoryStream();
//await PlayMusicYouTubeCommand.StreamAudioFileFFMPEG(stream, memoryStream);

await client.StartAsync();