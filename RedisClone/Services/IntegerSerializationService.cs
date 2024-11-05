using RedisClone.Utils;

namespace RedisClone.Services;

public class IntegerSerializationService : ISerializationService
{
    public object Deserialize(char[] bytes, int start)
    {
        string s = StringUtils.ReadNextString(bytes, start + 1);
        if(s.Length == 0) throw new ArgumentException("Invalid expression");
        bool success = int.TryParse(s, out var result);
        if (!success) throw new ArgumentException("Received invalid argument as integer");
        return result;
    }
    
    public object Deserialize(char[] bytes)
    {
        string s = StringUtils.ReadNextString(bytes, 1);
        if(s.Length == 0) throw new ArgumentException("Invalid expression");
        bool success = int.TryParse(s, out var result);
        if (!success) throw new ArgumentException("Received invalid argument as integer");
        return result;
    }

    public char[] Serialize(object obj)
    {
        throw new NotSupportedException("Unsupported operation");
    }
}