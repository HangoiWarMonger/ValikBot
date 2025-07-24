using Bot.Infrastructure.YouTube;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Bot.Tests;

public class YouTubeCookieUpdaterTests
{
    [Fact]
    public async Task StartAsync_WritesCookies()
    {
        var options = Options.Create(new YoutubeApiOptions
        {
            ApiKey = "dummy",
            YtDlpPath = Path.GetFullPath("yt-dlp"),
            Login = "valikyoutu1@gmail.com",
            Password = "valikvalik"
        });

        var updater = new YouTubeCookieUpdater(options, NullLogger<YouTubeCookieUpdater>.Instance);
        var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));

        var cookiesFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cookies.txt");
        if (File.Exists(cookiesFile))
            File.Delete(cookiesFile);

        await updater.StartAsync(cts.Token);

        Assert.True(File.Exists(cookiesFile));
        Assert.True(new FileInfo(cookiesFile).Length > 0);
    }
}
