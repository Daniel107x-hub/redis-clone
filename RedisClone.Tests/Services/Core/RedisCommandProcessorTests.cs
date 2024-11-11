using Moq;
using RedisClone.Services.Core;
using RedisClone.Services.Time;

namespace RedisClone.Tests.Services.Core;

public class RedisCommandProcessorTests
{
    private readonly Mock<ITimeProvider> _timeProviderMock;
    
    public RedisCommandProcessorTests()
    {
        _timeProviderMock = new Mock<ITimeProvider>();
    }
    
    [Fact]
    public void ProcessCommand_PingCommand_ReturnsPong()
    {
        var processor = new RedisCommandProcessor(_timeProviderMock.Object);
        var result = processor.ProcessCommand("PING", new List<object>());
        Assert.Equal("PONG", result);
    }

    [Fact]
    public void ProcessCommand_EchoCommandWithArgument_ReturnsArgument()
    {
        var processor = new RedisCommandProcessor(_timeProviderMock.Object);
        var result = processor.ProcessCommand("ECHO", new List<object> { "Hello, World!" });
        Assert.Equal("Hello, World!", result);
    } 
    
    [Fact]
    public void ProcessCommand_EchoCommandWithNoArguments_ReturnsArgumentException()
    {
        var processor = new RedisCommandProcessor(_timeProviderMock.Object);
        var result = processor.ProcessCommand("ECHO", new List<object>());
        Assert.IsType<ArgumentException>(result);
    }
    
    [Fact]
    public void ProcessCommand_SetCommandWithTwoArguments_ReturnsOk()
    {
        var processor = new RedisCommandProcessor(_timeProviderMock.Object);
        var result = processor.ProcessCommand("SET", new List<object> { "key", "value" });
        Assert.Equal("OK", result);
    }
    
    [Fact]
    public void ProcessCommand_SetCommandWithAdditionalArguments_ReturnsOk()
    {
        var processor = new RedisCommandProcessor(_timeProviderMock.Object);
        var result = processor.ProcessCommand("SET", new List<object> { "key", "value", "EXP", "10" });
        Assert.Equal("OK", result);
    }
    
    [Fact]
    public void ProcessCommand_SetCommandWithLessThanTwoArguments_ReturnsArgumentException()
    {
        var processor = new RedisCommandProcessor(_timeProviderMock.Object);
        var result = processor.ProcessCommand("SET", new List<object> { "key" });
        Assert.IsType<ArgumentException>(result);
    }
    
    [Fact]
    public void ProcessCommand_GetCommandWithNoStoredKey_ReturnsNull()
    {
        var processor = new RedisCommandProcessor(_timeProviderMock.Object);
        var result = processor.ProcessCommand("GET", new List<object> { "key" });
        Assert.Null(result);
    }
    
    [Fact]
    public void ProcessCommand_GetCommandWithStoredKey_ReturnsValue()
    {
        var processor = new RedisCommandProcessor(_timeProviderMock.Object);
        processor.ProcessCommand("SET", new List<object> { "key", "value" });
        var result = processor.ProcessCommand("GET", new List<object> { "key" });
        Assert.Equal("value", result);
    }
    
    [Fact]
    public void ProcessCommand_GetCommandWithLessThanOneArgument_ReturnsArgumentException()
    {
        var processor = new RedisCommandProcessor(_timeProviderMock.Object);
        var result = processor.ProcessCommand("GET", new List<object>());
        Assert.IsType<ArgumentException>(result);
    }
    
    [Fact]
    public void ProcessCommand_GetCommandBeforeExpirationTimeSetInSeconds_ReturnsValue()
    {
        var processor = new RedisCommandProcessor(_timeProviderMock.Object);
        var initialTime = DateTime.Now;
        _timeProviderMock.Setup(x => x.Now).Returns(initialTime);
        processor.ProcessCommand("SET", new List<object> { "key", "value", "EXP", "10" });
        _timeProviderMock.Setup(x => x.Now).Returns(initialTime.AddSeconds(5));
        var result = processor.ProcessCommand("GET", new List<object> { "key" });
        Assert.Equal("value", result);
    }
    
    [Fact]
    public void ProcessCommand_GetCommandAfterExpirationTimeSetInSeconds_ReturnsNull()
    {
        var processor = new RedisCommandProcessor(_timeProviderMock.Object);
        var initialTime = DateTime.Now;
        _timeProviderMock.Setup(x => x.Now).Returns(initialTime);
        processor.ProcessCommand("SET", new List<object> { "key", "value", "EXP", "10" });
        _timeProviderMock.Setup(x => x.Now).Returns(initialTime.AddSeconds(15));
        var result = processor.ProcessCommand("GET", new List<object> { "key" });
        Assert.Null(result);
    }
    
    [Fact]
    public void ProcessCommand_GetCommandBeforeExpirationTimeSetInMilliseconds_ReturnsValue()
    {
        var processor = new RedisCommandProcessor(_timeProviderMock.Object);
        var initialTime = DateTime.Now;
        _timeProviderMock.Setup(x => x.Now).Returns(initialTime);
        processor.ProcessCommand("SET", new List<object> { "key", "value", "PX", "10000" });
        _timeProviderMock.Setup(x => x.Now).Returns(initialTime.AddMilliseconds(5000));
        var result = processor.ProcessCommand("GET", new List<object> { "key" });
        Assert.Equal("value", result);
    }
    
    [Fact]
    public void ProcessCommand_GetCommandAfterExpirationTimeSetInMilliseconds_ReturnsNull()
    {
        var processor = new RedisCommandProcessor(_timeProviderMock.Object);
        var initialTime = DateTime.Now;
        _timeProviderMock.Setup(x => x.Now).Returns(initialTime);
        processor.ProcessCommand("SET", new List<object> { "key", "value", "PX", "10000" });
        _timeProviderMock.Setup(x => x.Now).Returns(initialTime.AddMilliseconds(15000));
        var result = processor.ProcessCommand("GET", new List<object> { "key" });
        Assert.Null(result);
    }
    
    [Fact]
    public void ProcessCommand_GetCommandBeforeExpirationTimeSetInUnixSeconds_ReturnsValue()
    {
        var processor = new RedisCommandProcessor(_timeProviderMock.Object);
        var initialTime = DateTime.Now;
        var expirationDelay = 10;
        // Set expiration time to 10 seconds after initial time
        var expirationTime = new DateTimeOffset(initialTime.AddSeconds(expirationDelay));
        _timeProviderMock.Setup(x => x.FromUnixTimeSeconds(It.IsAny<long>())).Returns(expirationTime);
        processor.ProcessCommand("SET", new List<object> { "key", "value", "EXAT", expirationDelay.ToString() });
        _timeProviderMock.Verify(m => m.FromUnixTimeSeconds(It.Is<long>(x => x == expirationDelay)), Times.Once);
        // Set current time to 5 seconds after initial time
        _timeProviderMock.Setup(x => x.Now).Returns(initialTime.AddSeconds(5));
        var result = processor.ProcessCommand("GET", new List<object> { "key" });
        Assert.Equal("value", result);
    }
    
    [Fact]
    public void ProcessCommand_GetCommandAfterExpirationTimeSetInUnixSeconds_ReturnsNull()
    {
        var processor = new RedisCommandProcessor(_timeProviderMock.Object);
        var initialTime = DateTime.Now;
        var expirationDelay = 10;
        // Set expiration time to 10 seconds after initial time
        var expirationTime = new DateTimeOffset(initialTime.AddSeconds(expirationDelay));
        _timeProviderMock.Setup(x => x.FromUnixTimeSeconds(It.IsAny<long>())).Returns(expirationTime);
        processor.ProcessCommand("SET", new List<object> { "key", "value", "EXAT", expirationDelay.ToString() });
        _timeProviderMock.Verify(m => m.FromUnixTimeSeconds(It.Is<long>(x => x == expirationDelay)), Times.Once);
        // Set current time to 15 seconds after initial time
        _timeProviderMock.Setup(x => x.Now).Returns(initialTime.AddSeconds(15));
        var result = processor.ProcessCommand("GET", new List<object> { "key" });
        Assert.Null(result);
    }
    
    [Fact]
    public void ProcessCommand_GetCommandBeforeExpirationTimeSetInUnixMilliseconds_ReturnsValue()
    {
        var processor = new RedisCommandProcessor(_timeProviderMock.Object);
        var initialTime = DateTime.Now;
        var expirationDelay = 10000;
        // Set expiration time to 10 seconds after initial time
        var expirationTime = new DateTimeOffset(initialTime.AddMilliseconds(expirationDelay));
        _timeProviderMock.Setup(x => x.FromUnixTimeMilliseconds(It.IsAny<long>())).Returns(expirationTime);
        processor.ProcessCommand("SET", new List<object> { "key", "value", "PXAT", expirationDelay.ToString() });
        _timeProviderMock.Verify(m => m.FromUnixTimeMilliseconds(It.Is<long>(x => x == expirationDelay)), Times.Once);
        // Set current time to 5 seconds after initial time
        _timeProviderMock.Setup(x => x.Now).Returns(initialTime.AddMilliseconds(5000));
        var result = processor.ProcessCommand("GET", new List<object> { "key" });
        Assert.Equal("value", result);
    }
    
    [Fact]
    public void ProcessCommand_GetCommandAfterExpirationTimeSetInUnixMilliseconds_ReturnsNull()
    {
        var processor = new RedisCommandProcessor(_timeProviderMock.Object);
        var initialTime = DateTime.Now;
        var expirationDelay = 10000;
        // Set expiration time to 10 seconds after initial time
        var expirationTime = new DateTimeOffset(initialTime.AddMilliseconds(expirationDelay));
        _timeProviderMock.Setup(x => x.FromUnixTimeMilliseconds(It.IsAny<long>())).Returns(expirationTime);
        processor.ProcessCommand("SET", new List<object> { "key", "value", "PXAT", expirationDelay.ToString() });
        _timeProviderMock.Verify(m => m.FromUnixTimeMilliseconds(It.Is<long>(x => x == expirationDelay)), Times.Once);
        // Set current time to 15 seconds after initial time
        _timeProviderMock.Setup(x => x.Now).Returns(initialTime.AddMilliseconds(15000));
        var result = processor.ProcessCommand("GET", new List<object> { "key" });
        Assert.Null(result);
    }
}