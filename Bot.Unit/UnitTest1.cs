using Bot.Infrastructure.SoundCloud;
using Bot.Infrastructure.YouTube;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bot.Unit;

public class UnitTest1
{
    private const string TestUrl = "https://soundcloud.com/ghostemane/ghostemane-kybalion-prod-nedarb";

    [Fact]
    public async Task SuccessfulGetTrackInfo()
    {
        using var client = new HttpClient();
        var trackClient = new SoundCloudTrackClient(client, "TeyyF4yyJvOCHG1txg8Z4jfrHt8gcLzc");

        var info = await trackClient.GetTracksFromLink("https://soundcloud.com/g59/sets/new-world-depression");

        Assert.NotNull(info);
    }

    [Fact]
    public async Task SuccessfulGetTrackStream()
    {
        using var client = new HttpClient();
        var yt = new YoutubeApiOptions
        {
            ApiKey = "AIzaSyBA1kGEpa8amv8jclJutG5-P9NWLfdlr7Y",
            YtDlpPath = "sda"
        };
        var options = Options.Create(yt);
        var logger = new LoggerFactory().CreateLogger<YouTubeTrackClient>();
        var trackClient = new YouTubeTrackClient(options, logger);

        var info = await trackClient.GetAudioStreamAsync("https://www.youtube.com/watch?v=I59V9pjEOLI");

        Assert.NotNull(info);
    }
}