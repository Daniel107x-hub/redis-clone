using RedisClone.Utils;

namespace RedisClone.Services;

public class BulkStringSerializationService : ISerializationService
{
    public object Deserialize(char[] bytes, int start)
    {
        int i = start + 1;
        string s = StringUtils.ReadNextString(bytes, i);
        if (s == "-1") return null;
        int.TryParse(s, out var length);
        if (length == 0) return "";
        if(bytes.Length < "$\r\n\r\n".Length + s.Length + length) throw new ArgumentException("Invalid string");
        i += s.Length + 2;
        s = StringUtils.ReadNextString(bytes, i);
        return s;
    }
    
    public object Deserialize(char[] bytes)
    {
        int i = 1;
        string s = StringUtils.ReadNextString(bytes, i);
        if (s == "-1") return null;
        int.TryParse(s, out var length);
        if (length == 0) return "";
        if(bytes.Length < "$\r\n\r\n".Length + s.Length + length) throw new ArgumentException("Invalid string");
        i += s.Length + 2;
        s = StringUtils.ReadNextString(bytes, i);
        return s;
    }

    public char[] Serialize(object obj)
    {
        throw new NotImplementedException();
    }
}