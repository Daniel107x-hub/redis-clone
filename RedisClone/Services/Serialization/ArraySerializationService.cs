using System.Text;

namespace RedisClone.Services.Serialization;

public class ArraySerializationService : ISerializationService
{
    public object Deserialize(StreamReader reader)
    {
        StringBuilder sb = new();
        char prev = '\0';
        List<object> result = new();
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
        if (length == 0) return result;
        if (reader.EndOfStream) throw new ArgumentException("Invalid format");
        while (length > 0)
        {
            char type = (char)reader.Read();
            ISerializationService service = SerializationServiceFactory.GetSerializationService(type);
            object obj = service.Deserialize(reader);
            result.Add(obj);
            length--;
        }
        return result;
    }

    public byte[] Serialize(object obj)
    {
        throw new NotImplementedException();
    }
}