using Bot.Discord.Common.DependencyInjection;
using Bot.Infrastructure.SoundCloud;
using Bot.Infrastructure.YouTube;
using Bot.Domain.Exceptions;
using Xunit;

namespace Bot.Tests;

public class TrackClientFactoryTests
{
    [Fact]
    public void Get_ReturnsYouTubeClient_ForYouTubeUrl()
    {
        var ytClient = new YouTubeTrackClient(new HttpClient(), Microsoft.Extensions.Options.Options.Create(new YoutubeApiOptions{ApiKey="key"}), Microsoft.Extensions.Logging.Abstractions.NullLogger<YouTubeTrackClient>.Instance);
        var scClient = new SoundCloudTrackClient(new HttpClient(), "id");
        var factory = new TrackClientFactory(ytClient, scClient);

        var client = factory.Get("https://www.youtube.com/watch?v=abc123");

        Assert.Same(ytClient, client);
    }

    [Fact]
    public void Get_Throws_For_UnsupportedUrl()
    {
        var ytClient = new YouTubeTrackClient(new HttpClient(), Microsoft.Extensions.Options.Options.Create(new YoutubeApiOptions{ApiKey="key"}), Microsoft.Extensions.Logging.Abstractions.NullLogger<YouTubeTrackClient>.Instance);
        var scClient = new SoundCloudTrackClient(new HttpClient(), "id");
        var factory = new TrackClientFactory(ytClient, scClient);

        Assert.Throws<UnsupportedTrackSourceException>(() => factory.Get("https://example.com"));
    }
    [Fact]
    public void Get_ReturnsSoundCloudClient_ForSoundCloudUrl()
    {
        var ytClient = new YouTubeTrackClient(new HttpClient(), Microsoft.Extensions.Options.Options.Create(new YoutubeApiOptions{ApiKey="key"}), Microsoft.Extensions.Logging.Abstractions.NullLogger<YouTubeTrackClient>.Instance);
        var scClient = new SoundCloudTrackClient(new HttpClient(), "id");
        var factory = new TrackClientFactory(ytClient, scClient);

        var client = factory.Get("https://soundcloud.com/user/track");

        Assert.Same(scClient, client);
    }
}
