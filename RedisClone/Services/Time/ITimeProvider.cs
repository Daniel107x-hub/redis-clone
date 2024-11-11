namespace RedisClone.Services.Time;

public interface ITimeProvider
{
    public DateTime Now { get; }
    public DateTimeOffset FromUnixTimeSeconds(long seconds);
    public DateTimeOffset FromUnixTimeMilliseconds(long milliseconds);
}