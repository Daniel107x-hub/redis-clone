using System.Text;
using RedisClone.Services.Serialization;

namespace RedisClone.Tests.Services.Serialization;

public class ArraySerializationServiceTests
{
    [Fact]
    public void Deserialize_WhenCalledWithLengthZero_ReturnsEmptyArray()
    {
        string input = "0\r\n";
        byte[] bytes = Encoding.ASCII.GetBytes(input);
        using MemoryStream stream = new(bytes);
        ArraySerializationService service = new();
        using StreamReader reader = new(stream);
        var result = (List<object>)service.Deserialize(reader);
        Assert.Empty(result);
    }
    
    [Fact]
    public void Deserialize_WhenCalledWithValidArrayOfStrings_ReturnsArray()
    {
        string input = "3\r\n$3\r\nfoo\r\n$3\r\nbar\r\n$3\r\nbaz\r\n";
        byte[] bytes = Encoding.ASCII.GetBytes(input);
        using MemoryStream stream = new(bytes);
        ArraySerializationService service = new();
        using StreamReader reader = new(stream);
        var result = service.Deserialize(reader);
        Assert.Equal(new object[] { "foo", "bar", "baz" }, result);
    }
    
    [Fact]
    public void Deserialize_WhenCalledWithInvalidArrayOfStrings_ThrowsArgumentException()
    {
        string input = "3\r\n$3\r\nfoo\r\n$3\r\nbar\r\n$3\r\nbaz";
        byte[] bytes = Encoding.ASCII.GetBytes(input);
        using MemoryStream stream = new(bytes);
        ArraySerializationService service = new();
        using StreamReader reader = new(stream);
        Assert.Throws<ArgumentException>(() => service.Deserialize(reader));
    }
    
    [Fact]
    public void Deserialize_WhenCalledWithValidArrayOfIntegers_ReturnsArray()
    {
        string input = "3\r\n:1\r\n:2\r\n:3\r\n";
        byte[] bytes = Encoding.ASCII.GetBytes(input);
        using MemoryStream stream = new(bytes);
        ArraySerializationService service = new();
        using StreamReader reader = new(stream);
        var result = service.Deserialize(reader);
        Assert.Equal(new int[] { 1, 2, 3 }, result);
    }
    
    [Fact]
    public void Deserialize_WhenCalledWithInvalidArrayOfIntegers_ThrowsArgumentException()
    {
        string input = "3\r\n:1\r\n:2\r\n:3";
        byte[] bytes = Encoding.ASCII.GetBytes(input);
        using MemoryStream stream = new(bytes);
        ArraySerializationService service = new();
        using StreamReader reader = new(stream);
        Assert.Throws<ArgumentException>(() => service.Deserialize(reader));
    }
    
    [Fact]
    public void Deserialize_WhenCalledWithValidArrayOfMixedTypes_ReturnsArray()
    {
        string input = "8\r\n:1\r\n:2\r\n:3\r\n$3\r\nfoo\r\n+OK\r\n:1\r\n-Error message\r\n+TEST\r\n";
        byte[] bytes = Encoding.ASCII.GetBytes(input);
        using MemoryStream stream = new(bytes);
        ArraySerializationService service = new();
        using StreamReader reader = new(stream);
        var result = (List<object>)service.Deserialize(reader);
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
        string input = "*5\r\n:1\r\n:2\r\n:3\r\n$3\r\nfoo\r\n+OK";
        byte[] bytes = Encoding.ASCII.GetBytes(input);
        using MemoryStream stream = new(bytes);
        ArraySerializationService service = new();
        using StreamReader reader = new(stream);
        Assert.Throws<ArgumentException>(() => service.Deserialize(reader));
    }
    
    [Fact]
    public void Serialize_WhenCalledWithAListOfIntegers_SerializesList()
    {
        ArraySerializationService service = new();
        List<object> list = new() { 1, 2, 3 };
        byte[] result = service.Serialize(list);
        string expected = "*3\r\n:1\r\n:2\r\n:3\r\n";
        byte[] bytes = Encoding.ASCII.GetBytes(expected);
        Assert.Equal(bytes, result);
    }
    
    [Fact]
    public void Serialize_WhenCalledWithAListOfStrings_SerializesList()
    {
        ArraySerializationService service = new();
        List<object> list = new() { "f\\o", "bar", "baz" };
        byte[] result = service.Serialize(list);
        string expected = "*3\r\n$3\r\nf\\o\r\n+bar\r\n+baz\r\n";
        byte[] bytes = Encoding.ASCII.GetBytes(expected);
        Assert.Equal(bytes, result);
    }
    
    [Fact]
    public void Serialize_WhenCalledWithAListOfMixedTypes_SerializesList()
    {
        ArraySerializationService service = new();
        List<object> list = new() { 1, "f\ro", "OK", 1, new Exception("Error message"), "TEST" };
        byte[] result = service.Serialize(list);
        string expected = "*6\r\n:1\r\n$3\r\nf\ro\r\n+OK\r\n:1\r\n-Error message\r\n+TEST\r\n";
        byte[] bytes = Encoding.ASCII.GetBytes(expected);
        Assert.Equal(bytes, result);
    }
    
    [Fact]
    public void Serialize_WhenCalledWithEmptyList_SerializesList()
    {
        ArraySerializationService service = new();
        List<object> list = new();
        byte[] result = service.Serialize(list);
        string expected = "*0\r\n";
        byte[] bytes = Encoding.ASCII.GetBytes(expected);
        Assert.Equal(bytes, result);
    }
    
    [Fact]
    public void Serialize_WhenCalledWithNull_ThrowsArgumentNullException()
    {
        ArraySerializationService service = new();
        Assert.Throws<ArgumentNullException>(() => service.Serialize(null));
    }
}