using System.Text;

namespace RedisClone.Services.Serialization;

public class ExceptionSerializationService : ISerializationService
{
    public object Deserialize(StreamReader reader)
    {
        StringBuilder sb = new();
        char prev = '\0';
        while (reader.Peek() != -1)
        {
            char current = (char)reader.Read();
            if (current == '\n' && prev == '\r') return new Exception(sb.ToString());
            prev = current;
            if (current == '\r') continue;
            sb.Append(current);
        }
        throw new ArgumentException("Invalid format");
    }

    public byte[] Serialize(object obj)
    {
        if (!(obj is Exception)) throw new ArgumentException("Invalid type");
        Exception exception = (Exception)obj;
        return Encoding.ASCII.GetBytes($"-{exception.Message}\r\n");
    }
}