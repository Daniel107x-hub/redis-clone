using System.Text;
using RedisClone.Services.Serialization;

namespace RedisClone.Tests.Services.Serialization;

public class ExceptionSerializationServiceTests
{
    [Fact]
    public void Deserialize_WhenCalledWithValidError_ReturnsException()
    {
        string input = "ERR unknown command 'foobar'\r\n";
        byte[] bytes = Encoding.ASCII.GetBytes(input);
        using MemoryStream stream = new(bytes);
        ExceptionSerializationService serializationService = new();
        using StreamReader reader = new(stream);
        var result = (Exception)serializationService.Deserialize(reader);
        Assert.Equal("ERR unknown command 'foobar'", result.Message);
    }
    
    [Theory]
    [InlineData("ERR unknown command 'foobar'")]
    [InlineData("ERR unknown command 'foobar'\\n")]
    public void Deserialize_WhenCalledWithInvalidError_ThrowsArgumentException(string input)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(input);
        using MemoryStream stream = new(bytes);
        ExceptionSerializationService serializationService = new();
        using StreamReader reader = new(stream);
        Assert.Throws<ArgumentException>(() => serializationService.Deserialize(reader));
    }
}