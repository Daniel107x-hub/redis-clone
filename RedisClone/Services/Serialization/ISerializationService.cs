namespace RedisClone.Services.Serialization;

public interface ISerializationService
{
    object Deserialize(StreamReader reader);
    byte[] Serialize(object obj);
}