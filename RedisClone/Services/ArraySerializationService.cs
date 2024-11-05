using RedisClone.Utils;

namespace RedisClone.Services;

public class ArraySerializationService : ISerializationService
{
    public object Deserialize(char[] bytes, int start)
    {
        throw new NotImplementedException();
    }

    public object Deserialize(char[] bytes)
    {
        int i = 1;
        string s = StringUtils.ReadNextString(bytes, i);
        bool success = int.TryParse(s, out int length);
        if(!success) throw new ArgumentException("Invalid format");
        if(length == -1) return null;
        i += s.Length + 2;
        List<object> list = new();
        SerializationService service = new();
        for (int j = 0; j < length; j++)
        {
            object obj = service.Deserialize(bytes, i);
            list.Add(obj);
            if (obj == null) i += 5;
            else if(obj is int || bytes[i] == '+') i+= obj.ToString().Length + 3;
            else if(obj is Exception ex) i += ex.Message.Length + 3;
            else i += obj.ToString().Length + obj.ToString().Length.ToString().Length + 5;
        }

        return list;
    }

    public char[] Serialize(object obj)
    {
        throw new NotImplementedException();
    }
}