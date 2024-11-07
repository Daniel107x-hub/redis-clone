using System.Text;
using RedisClone.Services.Serialization;

namespace RedisClone.Tests.Services.Serialization;

public class IntegerSerializationServiceTests 
{
    
    [Fact]
    public void Serialize_WhenCalledWithInteger_ReturnsBytes()
    {
        IntegerSerializationService integerSerializationService = new();
        int input = 1000;
        byte[] expected = Encoding.ASCII.GetBytes(":1000\r\n");
        var result = integerSerializationService.Serialize(input);
        Assert.Equal(expected, result);
    }
    
    [Fact]
    public void Serialize_WhenCalledWithInvalidType_ThrowsArgumentException()
    {
        IntegerSerializationService integerSerializationService = new();
        Assert.Throws<ArgumentException>(() => integerSerializationService.Serialize("1000"));
    }
    
    [Theory]
    [InlineData("10000\r\n", 10000)]
    [InlineData("1000\r\n", 1000)]
    [InlineData("+100\r\n", 100)]
    [InlineData("10\r\n", 10)]
    [InlineData("-1\r\n", -1)]
    public void Deserialize_WhenValledWithValidInteger_ReturnsInteger(string input, int expectedInt)
    {
        IntegerSerializationService integerSerializationService = new();
        byte[] bytes = Encoding.ASCII.GetBytes(input);
        using MemoryStream stream = new(bytes);
        using StreamReader reader = new(stream);
        var result = integerSerializationService.Deserialize(reader);
        Assert.Equal(expectedInt, result);
    }
    
    [Theory]
    [InlineData("\rn")]
    [InlineData("10000\n")]
    [InlineData("abdg\r\n")]
    public void Deserialize_WhenCalledWithInvalidInteger_ThrowsArgumentException(string input)
    {
        IntegerSerializationService integerSerializationService = new();
        byte[] bytes = Encoding.ASCII.GetBytes(input);
        using MemoryStream stream = new(bytes);
        using StreamReader reader = new(stream);
        Assert.Throws<ArgumentException>(() => integerSerializationService.Deserialize(reader));
    }
}