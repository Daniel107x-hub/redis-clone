using RedisClone.Services;

namespace RedisClone.Tests.Services;

public class SerializationServiceTests
{
    [Fact]
    public void Deserialize_WhenCalledWithNullString_ReturnsNull()
    {
        SerializationService serializationService = new();
        string input = "$-1\r\n";
        var result = serializationService.Deserialize(input.ToCharArray());
        Assert.Null(result);
    }
    
    [Fact]
    public void Deserialize_WhenCalledWithLengthZero_ReturnsEmptyString()
    {
        SerializationService serializationService = new();
        string input = "$0\r\n\r\n";
        var result = serializationService.Deserialize(input.ToCharArray());
        Assert.Equal("", result);
    }
    
    [Fact]
    public void Deserialize_WhenCalledWithInvalidFormat_ThrowsArgumentException()
    {
        SerializationService serializationService = new();
        string input = "$0\r";
        Assert.Throws<ArgumentException>(() => serializationService.Deserialize(input.ToCharArray()));
    }
    
    [Fact]
    public void Deserialize_WhenCalledWithInvalidString_ThrowsArgumentException()
    {
        SerializationService serializationService = new();
        string input = "$5\r\nhello";
        Assert.Throws<ArgumentException>(() => serializationService.Deserialize(input.ToCharArray()));
    }
    
    [Fact]
    public void Deserialize_WhenCalledWithShorterString_ThrowsArgumentException()
    {
        SerializationService serializationService = new();
        string input = "$5\r\nheo\r\n";
        Assert.Throws<ArgumentException>(() => serializationService.Deserialize(input.ToCharArray()));
    }
    
    [Fact]
    public void Deserialize_WhenCalledWithValidString_ReturnsString()
    {
        SerializationService serializationService = new();
        string input = "$5\r\nhello\r\n";
        var result = serializationService.Deserialize(input.ToCharArray());
        Assert.Equal("hello", result);
    }

    [Fact]
    public void Deserialize_WhenCalledWithASimpleString_ReturnsString()
    {
        SerializationService serializationService = new();
        string input = "+OK\r\n";
        var result = serializationService.Deserialize(input.ToCharArray());
        Assert.Equal("OK", result);
    }

    [Fact]
    public void Deserialize_WhenCalledWithInvalidFormatAsSimpleString_ThrowsArgumentException()
    {
        SerializationService serializationService = new();
        string input = "+OK\\n";
        Assert.Throws<ArgumentException>(() => serializationService.Deserialize(input.ToCharArray()));
    }
    
    [Fact]
    public void Deserialize_WhenCalledWithInvalidInteger_ThrowsArgumentException()
    {
        SerializationService serializationService = new();
        string input = ":hello\r\n";
        Assert.Throws<ArgumentException>(() => serializationService.Deserialize(input.ToCharArray()));
    }
    
    [Fact]
    public void Deserialize_WhenCalledWithValidInteger_ReturnsInteger()
    {
        SerializationService serializationService = new();
        string input = ":123\r\n";
        var result = serializationService.Deserialize(input.ToCharArray());
        Assert.Equal(123, result);
    }
    
    [Fact]
    public void Deserialize_WhenCalledWithValidError_ThrowsException()
    {
        SerializationService serializationService = new();
        string input = "-Error message\r\n";
        var exception = (Exception) serializationService.Deserialize(input.ToCharArray());
        Assert.Equal("Error message", exception.Message);
    }
    
    [Fact]
    public void Deserialize_WhenCalledWithInvalidError_ThrowsArgumentException()
    {
        SerializationService serializationService = new();
        string input = "-Error message";
        Assert.Throws<ArgumentException>(() => serializationService.Deserialize(input.ToCharArray()));
    }
    
    [Fact]
    public void Deserialize_WhenCalledWithValidArrayOfStrings_ReturnsArray()
    {
        SerializationService serializationService = new();
        string input = "*3\r\n$3\r\nfoo\r\n$3\r\nbar\r\n$3\r\nbaz\r\n";
        var result = serializationService.Deserialize(input.ToCharArray());
        Assert.Equal(new object[] { "foo", "bar", "baz" }, result);
    }
    
    [Fact]
    public void Deserialize_WhenCalledWithInvalidArrayOfStrings_ThrowsArgumentException()
    {
        SerializationService serializationService = new();
        string input = "*3\r\n$3\r\nfoo\r\n$3\r\nbar\r\n$3\r\nbaz";
        Assert.Throws<ArgumentException>(() => serializationService.Deserialize(input.ToCharArray()));
    }
    
    [Fact]
    public void Deserialize_WhenCalledWithValidArrayOfIntegers_ReturnsArray()
    {
        SerializationService serializationService = new();
        string input = "*3\r\n:1\r\n:2\r\n:3\r\n";
        var result = serializationService.Deserialize(input.ToCharArray());
        Assert.Equal(new int[] { 1, 2, 3 }, result);
    }
    
    [Fact]
    public void Deserialize_WhenCalledWithInvalidArrayOfIntegers_ThrowsArgumentException()
    {
        SerializationService serializationService = new();
        string input = "*3\r\n:1\r\n:2\r\n:3";
        Assert.Throws<ArgumentException>(() => serializationService.Deserialize(input.ToCharArray()));
    }
    
    [Fact]
    public void Deserialize_WhenCalledWithValidArrayOfMixedTypes_ReturnsArray()
    {
        SerializationService serializationService = new();
        string input = "*8\r\n:1\r\n:2\r\n:3\r\n$3\r\nfoo\r\n+OK\r\n:1\r\n-Error message\r\n+TEST\r\n";
        var result = (List<object>)serializationService.Deserialize(input.ToCharArray());
        Assert.Equal(result[0], 1);
        Assert.Equal(result[1], 2);
        Assert.Equal(result[2], 3);
        Assert.Equal(result[3], "foo");
        Assert.Equal(result[4], "OK");
        Assert.Equal(result[5], 1);
        Assert.Equal(((Exception)result[6]).Message, "Error message");
        Assert.Equal(result[7], "TEST");
    }
    
    [Fact]
    public void Deserialize_WhenCalledWithInvalidArrayOfMixedTypes_ThrowsArgumentException()
    {
        SerializationService serializationService = new();
        string input = "*5\r\n:1\r\n:2\r\n:3\r\n$3\r\nfoo\r\n+OK";
        Assert.Throws<ArgumentException>(() => serializationService.Deserialize(input.ToCharArray()));
    }
}