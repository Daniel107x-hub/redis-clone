using System.Text;

namespace RedisClone.Services.Serialization;

public class BulkStringSerializationService : ISerializationService
{
    public object Deserialize(StreamReader reader)
    {
        char prev = '\0';
        StringBuilder sb = new();
        while (reader.Peek() != -1)
        {
            char current = (char)reader.Read();
            if (current == '\n' && prev == '\r') break;
            prev = current;
            if (current == '\r') continue;
            sb.Append(current);
        }
        bool success = int.TryParse(sb.ToString(), out int length);
        if(!success) throw new ArgumentException("Invalid format");
        if (length == -1) return null;
        if (length == 0) return "";
        sb = new();
        while(length > 0)
        {
            if (reader.Peek() == -1) throw new ArgumentException("Invalid format");
            char current = (char)reader.Read();
            sb.Append(current);
            length--;
        }
        if(reader.Read() != '\r' || reader.Read() != '\n') throw new ArgumentException("Invalid format");
        return sb.ToString();
    }

    public byte[] Serialize(object obj)
    {
        if(obj == null) return Encoding.ASCII.GetBytes("$-1\r\n");
        if(!(obj is string)) throw new ArgumentException("Invalid type");
        string str = obj.ToString();
        return Encoding.ASCII.GetBytes($"${str.Length}\r\n{str}\r\n");
    }
}