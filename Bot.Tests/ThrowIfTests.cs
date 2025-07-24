using Bot.Domain.Validation;
using Xunit;

namespace Bot.Tests;

public class ThrowIfTests
{
    [Fact]
    public void NullOrWhiteSpace_Throws_OnNull()
    {
        Assert.Throws<ArgumentNullException>(() => ThrowIf.NullOrWhiteSpace(null, "arg"));
    }

    [Fact]
    public void NullOrWhiteSpace_Throws_OnWhitespace()
    {
        Assert.Throws<ArgumentNullException>(() => ThrowIf.NullOrWhiteSpace("   ", "arg"));
    }

    [Fact]
    public void NullOrWhiteSpace_DoesNotThrow_OnValid()
    {
        var ex = Record.Exception(() => ThrowIf.NullOrWhiteSpace("ok", "arg"));
        Assert.Null(ex);
    }

    [Fact]
    public void Null_Throws_OnNull()
    {
        Assert.Throws<ArgumentNullException>(() => ThrowIf.Null<object?>(null, "obj"));
    }

    [Fact]
    public void Null_DoesNotThrow_OnNotNull()
    {
        var ex = Record.Exception(() => ThrowIf.Null(new object(), "obj"));
        Assert.Null(ex);
    }
}
