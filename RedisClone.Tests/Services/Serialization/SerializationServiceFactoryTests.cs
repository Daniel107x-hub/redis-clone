using RedisClone.Services.Serialization;

namespace RedisClone.Tests.Services.Serialization;

public class SerializationServiceFactoryTests
{
    [Fact] 
    public void Serialization_WhenCalledWithNull_ReturnsBulkStringSerializationService()
    {
        // Act
        var serializationService = SerializationServiceFactory.GetSerializationService(null);
        // Assert
        Assert.IsType<BulkStringSerializationService>(serializationService);
    }
    
    [Fact]
    public void Serialization_WhenCalledWithSimpleString_ReturnsSimpleStringSerializationService()
    {
        // Act
        var serializationService = SerializationServiceFactory.GetSerializationService("OK");
        // Assert
        Assert.IsType<SimpleStringSerializationService>(serializationService);
    }
    
    [Fact]
    public void Serialization_WhenCalledWithComplexString_ReturnsBulkStringSerializationService()
    {
        // Act
        var serializationService = SerializationServiceFactory.GetSerializationService("O\\K\r\n");
        // Assert
        Assert.IsType<BulkStringSerializationService>(serializationService);
    }
    
    [Fact]
    public void Serialization_WhenCalledWithInteger_ReturnsIntegerSerializationService()
    {
        // Act
        var serializationService = SerializationServiceFactory.GetSerializationService(123);
        // Assert
        Assert.IsType<IntegerSerializationService>(serializationService);
    }
    
    [Fact]
    public void Serialization_WhenCalledWithException_ReturnsExceptionSerializationService()
    {
        // Act
        var serializationService = SerializationServiceFactory.GetSerializationService(new Exception());
        // Assert
        Assert.IsType<ExceptionSerializationService>(serializationService);
    }
    
    [Fact]
    public void Serialization_WhenCalledWithList_ReturnsArraySerializationService()
    {
        // Act
        var serializationService = SerializationServiceFactory.GetSerializationService(new List<object>());
        // Assert
        Assert.IsType<ArraySerializationService>(serializationService);
    }
    
    [Fact]
    public void Deserialization_WhenCalledWithSimpleStringType_ReturnsSimpleStringSerializationService()
    {
        // Act
        var serializationService = SerializationServiceFactory.GetSerializationService('+');
        // Assert
        Assert.IsType<SimpleStringSerializationService>(serializationService);
    }
    
    [Fact]
    public void Deserialization_WhenCalledWithBulkStringType_ReturnsBulkStringSerializationService()
    {
        // Act
        var serializationService = SerializationServiceFactory.GetSerializationService('$');
        // Assert
        Assert.IsType<BulkStringSerializationService>(serializationService);
    }
    
    [Fact]
    public void Deserialization_WhenCalledWithIntegerType_ReturnsIntegerSerializationService()
    {
        // Act
        var serializationService = SerializationServiceFactory.GetSerializationService(':');
        // Assert
        Assert.IsType<IntegerSerializationService>(serializationService);
    }
    
    [Fact]
    public void Deserialization_WhenCalledWithExceptionType_ReturnsExceptionSerializationService()
    {
        // Act
        var serializationService = SerializationServiceFactory.GetSerializationService('-');
        // Assert
        Assert.IsType<ExceptionSerializationService>(serializationService);
    }
    
    [Fact]
    public void Deserialization_WhenCalledWithArrayType_ReturnsArraySerializationService()
    {
        // Act
        var serializationService = SerializationServiceFactory.GetSerializationService('*');
        // Assert
        Assert.IsType<ArraySerializationService>(serializationService);
    }
    
    [Fact]
    public void Deserialization_WhenCalledWithUnsupportedType_ThrowsNotSupportedException()
    {
        // Act
        void Act() => SerializationServiceFactory.GetSerializationService('!');
        // Assert
        Assert.Throws<NotSupportedException>(Act);
    }
    
}