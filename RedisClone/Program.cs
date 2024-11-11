using RedisClone.Services.Core;
using RedisClone.Services.Time;

namespace RedisClone
{
    class Program
    {
        static void Main(string[] args)
        {
            ITimeProvider timeProvider = new RealTimeProvider();
            IProcessor commandProcessor = new RedisCommandProcessor(timeProvider);
            AsyncRedisServer server = new(commandProcessor);
            server.StartAsync().Wait();
        }
    }
}