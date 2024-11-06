using System.Text;

namespace RedisClone.Services.Serialization;

public class IntegerSerializationService : ISerializationService
{
    public object Deserialize(StreamReader reader)
    {
        StringBuilder sb = new();
        char prev = '\0';
        while (reader.Peek() != -1)
        {
            char current = (char)reader.Read();
            if (current == '\n' && prev == '\r')
            {
                bool success = int.TryParse(sb.ToString(), out int value);
                if (success) return value;
                throw new ArgumentException("Invalid format");
            }
            prev = current;
            if (current == '\r') continue;
            sb.Append(current);
        }  
        throw new ArgumentException("Invalid format");
    }

    public byte[] Serialize(object obj)
    {
        throw new NotImplementedException();
    }
}