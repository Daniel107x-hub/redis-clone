namespace RedisClone.Services.Serialization;

public class SerializationServiceFactory
{
    public static ISerializationService GetSerializationService(char type)
    {
        ISerializationService serializationService;
        switch (type)
        {
            case '+':
                serializationService = new SimpleStringSerializationService();
                break;
            
            case '$':
                serializationService = new BulkStringSerializationService();
                break;
            
            case ':':
                serializationService = new IntegerSerializationService();
                break;
            
            case '-':
                serializationService = new ExceptionSerializationService();
                break;
            
            case '*':
                serializationService = new ArraySerializationService();
                break;
            
            default:
                throw new NotSupportedException("Operation not supported");
        }

        return serializationService;
    }
    
    public static ISerializationService GetSerializationService(object obj)
    {
        ISerializationService serializationService;
        switch (obj)
        {
            // TODO: Add serialization for bulk string and null
            case string _:
                serializationService = new SimpleStringSerializationService();
                break;
            
            case int _:
                serializationService = new IntegerSerializationService();
                break;
            
            case Exception _:
                serializationService = new ExceptionSerializationService();
                break;
            
            case string[] _:
                serializationService = new ArraySerializationService();
                break;
            
            default:
                throw new NotSupportedException("Operation not supported");
        }

        return serializationService;
    }
}