using System.Text;

namespace RedisClone.Services.Serialization;

public class SimpleStringSerializationService : ISerializationService
{
    public object Deserialize(StreamReader reader)
    {
        StringBuilder sb = new();
        char prev = '\0';
        while (reader.Peek() != -1)
        {
            char current = (char)reader.Read();
            if (current == '\n' && prev == '\r') return sb.ToString();
            prev = current;
            if (current == '\r') continue;
            sb.Append(current);
        }  
        throw new ArgumentException("Invalid format");
    }

    public byte[] Serialize(object obj)
    {
        if(!(obj is string)) throw new ArgumentException("Invalid type");
        string str = (string)obj;
        if(str.Contains('\r') || str.Contains('\n')) throw new ArgumentException("Invalid format");
        return Encoding.ASCII.GetBytes($"+{str}\r\n");
    }
}