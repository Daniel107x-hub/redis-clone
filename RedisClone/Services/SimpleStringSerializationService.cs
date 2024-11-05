using RedisClone.Utils;

namespace RedisClone.Services;

public class SimpleStringSerializationService : ISerializationService
{
    public object Deserialize(char[] bytes, int start)
    {
        string s = StringUtils.ReadNextString(bytes, start + 1);
        return s;
    }
    
    public object Deserialize(char[] bytes)
    {
        string s = StringUtils.ReadNextString(bytes, 1);
        return s;
    }

    public char[] Serialize(object obj)
    {
        throw new NotImplementedException();
    }
}