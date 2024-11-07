using System.Text;
using RedisClone.Services.Serialization;

namespace RedisClone.Tests.Services.Serialization;

public class SimpleStringSerializationServiceTests
{
    [Fact]
    public void Serialize_WithSimpleString_ReturnsBytes()
    {
        // Arrange
        string input = "OK";
        string expected = "+OK\r\n";
        SimpleStringSerializationService service = new();
        // Act

        byte[] result = service.Serialize(input);
        string resultString = Encoding.ASCII.GetString(result);
        // Assert
        Assert.Equal(expected, resultString);
    }
    
    [Fact]
    public void Serialize_WithInvalidType_ThrowsArgumentException()
    {
        // Arrange
        SimpleStringSerializationService service = new();
        // Act and Assert
        Assert.Throws<ArgumentException>(() => service.Serialize(1));
    }
    
    [Fact]
    public void Serialize_WithInvalidString_ThrowsArgumentException()
    {
        // Arrange
        SimpleStringSerializationService service = new();
        // Act and Assert
        Assert.Throws<ArgumentException>(() => service.Serialize("OK\r\n"));
    }
    
    [Fact]
    public void Deserialize_WithStreamContainingSimpleString_ReturnsString()
    {
        // Arrange
        string input = "OK\r\n";
        byte[] bytes = Encoding.ASCII.GetBytes(input);
        using MemoryStream stream = new(bytes);
        SimpleStringSerializationService service = new();
        using StreamReader reader = new(stream);
        
        // Act
        string result = (string)service.Deserialize(reader);

        // Assert
        Assert.Equal("OK", result);
    }
    
    [Fact]
    public void Deserialize_WhenPassedAnInvalidString_ThrowsArgumentException()
    {
        // Arrange
        string input = "OK";
        byte[] bytes = Encoding.ASCII.GetBytes(input);
        using MemoryStream stream = new(bytes);
        SimpleStringSerializationService service = new();
        using StreamReader reader = new(stream);
        // Act and Assert
        Assert.Throws<ArgumentException>(() => service.Deserialize(reader));
    }

    [Fact]
    public void Deserialize_WhenMissesTerminationCharacter_ThrowsArgumentException()
    {
        // Arrange
        string input = "OK\\n";
        byte[] bytes = Encoding.ASCII.GetBytes(input);
        using MemoryStream stream = new(bytes);
        SimpleStringSerializationService service = new();
        using StreamReader reader = new(stream);
        //Act and assert
        Assert.Throws<ArgumentException>(() => service.Deserialize(reader));
    }
    
    
}