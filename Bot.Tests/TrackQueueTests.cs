using Bot.Domain.Entities;
using Xunit;

namespace Bot.Tests;

public class TrackQueueTests
{
    [Fact]
    public void EnqueueAndDequeue_WorksCorrectly()
    {
        var queue = new TrackQueue();
        var track = new MusicTrack("https://example.com");

        queue.Enqueue(track);

        Assert.True(queue.Any());
        var result = queue.TryDequeue(out var dequeued);
        Assert.True(result);
        Assert.Same(track, dequeued);
    }

    [Fact]
    public async Task SkipAsync_ClearsCurrentTrack_WhenQueueEmpty()
    {
        var queue = new TrackQueue();
        var track = new MusicTrack("https://example.com");
        queue.Enqueue(track);
        queue.TryDequeue(out _);
        queue.IsPlaying = true;

        await queue.SkipAsync();

        Assert.False(queue.IsPlaying);
        Assert.False(queue.Any());
    }

    [Fact]
    public void GetAll_ReturnsCurrentAndQueuedTracks()
    {
        var queue = new TrackQueue();
        var first = new MusicTrack("https://example.com/1");
        var second = new MusicTrack("https://example.com/2");
        var third = new MusicTrack("https://example.com/3");

        queue.Enqueue(first);
        queue.TryDequeue(out _); // set current track
        queue.Enqueue(second);
        queue.Enqueue(third);

        var all = queue.GetAll().ToArray();

        Assert.Equal(new[] { first, second, third }, all);
    }

    [Fact]
    public void Any_ReturnsTrue_WhenOnlyCurrentSong()
    {
        var queue = new TrackQueue();
        var track = new MusicTrack("https://example.com/only");
        queue.Enqueue(track);
        queue.TryDequeue(out _);

        Assert.True(queue.Any());
    }

    [Fact]
    public void Any_ReturnsFalse_WhenQueueEmpty()
    {
        var queue = new TrackQueue();
        Assert.False(queue.Any());
    }
}
