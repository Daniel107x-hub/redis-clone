namespace RedisClone.Services.Serialization;

public interface ISerializationService
{
    object Deserialize(StreamReader reader);
    Stream Serialize(object obj);
}