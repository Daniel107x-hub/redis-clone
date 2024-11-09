using RedisClone.Services.Core;

namespace RedisClone.Tests.Services.Core;

public class RedisCommandProcessorTests
{
    [Fact]
    public void ProcessCommand_CommandIsNull_ReturnsNull()
    {
        IProcessor processor = new RedisCommandProcessor();
        var response = processor.ProcessCommand(null, new List<object>());
        Assert.Null(response);
    }

    [Fact]
    public void ProcessCommand_WhenPingCommand_ReturnsPong()
    {
        IProcessor processor = new RedisCommandProcessor();
        var response = processor.ProcessCommand("PING", new List<object>());
        Assert.NotNull(response);
        Assert.Equal("PONG", response);
    }

    [Fact]
    public void ProcessCommand_WhenSetCommandWithNoArguments_ReturnsException()
    {
        IProcessor processor = new RedisCommandProcessor();
        var response = processor.ProcessCommand("SET", new List<object>());
        Assert.IsType<ArgumentException>(response);
    }

    [Fact]
    public void ProcessCommand_WhenSetCommandWithArguments_ReturnsOk()
    {
        IProcessor processor = new RedisCommandProcessor();
        var response = processor.ProcessCommand("SET", new List<object> { "key", "value" });
        Assert.NotNull(response);
        Assert.Equal("OK", response);
    }

    [Fact]
    public void ProcessCommand_WhenGetCommandWithStoredKey_ReturnsValue()
    {
        IProcessor processor = new RedisCommandProcessor();
        processor.ProcessCommand("SET", new List<object> { "key", "value" });
        var response = processor.ProcessCommand("GET", new List<object> { "key" });
        Assert.NotNull(response);
        Assert.Equal("value", response);
    }

    [Fact]
    public void ProcessCommand_WhenGetCommandWithNotStoredKey_ReturnsNull()
    {
        IProcessor processor = new RedisCommandProcessor();
        var response = processor.ProcessCommand("GET", new List<object>{ "key" });
        Assert.Null(response);
    }

    [Fact]
    public void ProcessCommand_OnSubsequentSetCommandForSameKeyAndGet_ReturnsLatest()
    {
        IProcessor processor = new RedisCommandProcessor();
        processor.ProcessCommand("SET", new List<object> { "key", "value" });
        var response = processor.ProcessCommand("GET", new List<object> { "key" });
        Assert.NotNull(response);
        Assert.Equal("value", response);
        processor.ProcessCommand("SET", new List<object> { "key", "newValue" });
        response = processor.ProcessCommand("GET", new List<object> { "key" });
        Assert.NotNull(response);
        Assert.Equal("newValue", response);
    }
}