using Bot.Domain.ValueObjects;
using Bot.Domain.Entities;
using Xunit;

namespace Bot.Tests;

public class TrackLinkTests
{
    [Fact]
    public void MusicTrack_Throws_On_Invalid_Url()
    {
        Assert.Throws<FluentValidation.ValidationException>(() => new MusicTrack("not a url"));
    }

    [Fact]
    public void Equals_ReturnsTrue_For_SameUrl()
    {
        var a = new MusicTrack("https://example.com/track");
        var b = new MusicTrack("https://example.com/track");

        Assert.Equal(a.Link, b.Link);
    }
}
