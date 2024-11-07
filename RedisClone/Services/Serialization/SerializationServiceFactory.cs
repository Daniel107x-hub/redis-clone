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
            case null:
                serializationService = new BulkStringSerializationService();
                break;
            
            case string _:
                string message = (string)obj;
                if (message.Contains('\r') || message.Contains('\n') || message.Contains('\\'))
                    serializationService = new BulkStringSerializationService();
                else serializationService = new SimpleStringSerializationService();
                break;
            
            case int _:
                serializationService = new IntegerSerializationService();
                break;
            
            case Exception _:
                serializationService = new ExceptionSerializationService();
                break;
            
            case List<object> _:
                serializationService = new ArraySerializationService();
                break;
            
            default:
                throw new NotSupportedException("Operation not supported");
        }

        return serializationService;
    }
}