namespace RedisClone.Services;

public interface ISerializationService
{
    object Deserialize(char[] bytes, int start);
    object Deserialize(char[] bytes);
    char[] Serialize(object obj);
}