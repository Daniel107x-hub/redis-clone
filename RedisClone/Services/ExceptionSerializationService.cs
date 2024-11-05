using RedisClone.Utils;

namespace RedisClone.Services;

public class ExceptionSerializationService : ISerializationService
{
    public object Deserialize(char[] bytes, int start)
    {
        string message = StringUtils.ReadNextString(bytes, start + 1);
        return new Exception(message);
    }
    
    public object Deserialize(char[] bytes)
    {
        string message = StringUtils.ReadNextString(bytes, 1);
        return new Exception(message);
    }

    public char[] Serialize(object obj)
    {
        throw new NotImplementedException();
    }
}