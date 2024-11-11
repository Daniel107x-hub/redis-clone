namespace RedisClone.Services.Time;

public class RealTimeProvider : ITimeProvider
{
    public DateTime Now => DateTime.Now;
    public DateTimeOffset FromUnixTimeSeconds(long seconds)
    {
        return DateTimeOffset.FromUnixTimeSeconds(seconds);
    }

    public DateTimeOffset FromUnixTimeMilliseconds(long milliseconds)
    {
        return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
    }
}