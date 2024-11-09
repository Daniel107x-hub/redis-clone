using RedisClone.Services.Core;

namespace RedisClone
{
    class Program
    {
        static void Main(string[] args)
        {
            IProcessor commandProcessor = new RedisCommandProcessor();
            AsyncRedisServer server = new(commandProcessor);
            server.StartAsync().Wait();
        }
    }
}