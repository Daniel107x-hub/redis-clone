using System.Text;
using RedisClone.Services.Serialization;

namespace RedisClone.Tests.Services.Serialization;

public class BulkStringSerializationServiceTests
{
    [Fact]
    public void Deserialize_WhenCalledWithNullString_ReturnsNull()
    {
        string input = "-1\r\n";
        byte[] bytes = Encoding.ASCII.GetBytes(input);
        using MemoryStream stream = new(bytes);
        BulkStringSerializationService service = new();
        using StreamReader reader = new(stream);
        Assert.Null(service.Deserialize(reader));
    }

    [Fact]
    public void Deserialize_WhenCalledWitLengthZero_ReturnsEmptyString()
    {
        string input = "0\r\n\r\n";
        byte[] bytes = Encoding.ASCII.GetBytes(input);
        using MemoryStream stream = new(bytes);
        BulkStringSerializationService service = new();
        using StreamReader reader = new(stream);
        Assert.Equal("", service.Deserialize(reader));
    }

    [Theory]
    [InlineData("a\r\ndfgergerg\r\n")]
    [InlineData("a\r\n")]
    [InlineData("5\r\nsdf\r\n")]
    [InlineData("5\r\nhello\\n")]
    public void Deserialize_WhenCalledWithInvalidFormat_ThrowsArgumentException(string input)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(input);
        using MemoryStream stream = new(bytes);
        using StreamReader reader = new(stream);
        BulkStringSerializationService service = new();
        Assert.Throws<ArgumentException>(() => service.Deserialize(reader));
    }
    
    [Fact]
    public void Deserialize_WhenCalledWithValidString_ReturnsString()
    {
        string input = "5\r\nhello\r\n";
        byte[] bytes = Encoding.ASCII.GetBytes(input);
        using MemoryStream stream = new(bytes);
        BulkStringSerializationService service = new();
        using StreamReader reader = new(stream);
        Assert.Equal("hello", service.Deserialize(reader));
    }

    [Fact]
    public void Deserialize_WhenCalledWithSpecialCharacters_ReturnsValidString()
    {
        string input = "8\r\nhe\nl\r\nlo\r\n";
        byte[] bytes = Encoding.ASCII.GetBytes(input);
        using MemoryStream stream = new(bytes);
        BulkStringSerializationService service = new();
        using StreamReader reader = new(stream);
        Assert.Equal("he\nl\r\nlo", service.Deserialize(reader));
    }
    
    [Fact]
    public void Deserialize_WhenCalledWithValidRandomString_ReturnsString()
    {
        string uid = Guid.NewGuid().ToString();
        string input = $"{uid.Length}\r\n{uid}\r\n";
        byte[] bytes = Encoding.ASCII.GetBytes(input);
        using MemoryStream stream = new(bytes);
        BulkStringSerializationService service = new();
        using StreamReader reader = new(stream);
        Assert.Equal(uid, service.Deserialize(reader));
    }
    
    [Fact]
    public void Serialize_WhenCalledWithNull_ReturnsNull()
    {
        BulkStringSerializationService service = new();
        byte[] result = service.Serialize(null);
        Assert.Equal(Encoding.ASCII.GetBytes("$-1\r\n"), result);
    }
    
    [Fact]
    public void Serialize_WhenCalledWithInvalidType_ThrowsArgumentException()
    {
        BulkStringSerializationService service = new();
        Assert.Throws<ArgumentException>(() => service.Serialize(1));
    }
    
    [Fact]
    public void Serialize_WhenCalledWithString_SerializesString()
    {
        BulkStringSerializationService service = new();
        byte[] result = service.Serialize("hello");
        Assert.Equal(Encoding.ASCII.GetBytes("$5\r\nhello\r\n"), result);
    }
}