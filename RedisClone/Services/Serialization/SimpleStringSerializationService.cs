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

    public Stream Serialize(object obj)
    {
        throw new NotImplementedException();
    }
}