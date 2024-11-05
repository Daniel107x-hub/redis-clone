namespace RedisClone.Services;

public class SerializationService
{
    public object Deserialize(char[] bytes, int start)
    {
        char type = bytes[start];
        if(!bytes[^2].Equals('\r') || !bytes[^1].Equals('\n'))
        {
            throw new ArgumentException("Invalid format");
        }
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

        return serializationService.Deserialize(bytes, start);
    }
    public object Deserialize(char[] bytes)
    {
        char type = bytes[0];
        if(!bytes[^2].Equals('\r') || !bytes[^1].Equals('\n'))
        {
            throw new ArgumentException("Invalid format");
        }
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

        return serializationService.Deserialize(bytes);
    }

    public char[] Serialize(object obj)
    {
        throw new NotImplementedException();
    }
}