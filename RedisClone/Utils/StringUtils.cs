using System.Text;

namespace RedisClone.Utils;

public class StringUtils
{
    public static string ReadNextString(char[] bytes, int start)
    {
        StringBuilder sb = new();
        for (int i = start; i < bytes.Length; i++)
        {
            if (i == bytes.Length - 1)
            {
                throw new ArgumentException("Invalid format");
            }
            if (bytes[i] == '\r' && bytes[i + 1] == '\n')
            {
                break;
            }
            sb.Append(bytes[i]);
        }
        return sb.ToString();
    }
}