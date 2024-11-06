using System.Text;
using RedisClone.Services.Serialization;

namespace RedisClone.Tests.Services.Serialization;

public class SimpleStringSerializationServiceTests
{
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