using Bot.Infrastructure.SoundCloud;

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
}