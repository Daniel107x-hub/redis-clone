namespace RedisClone
{
    class Program
    {
        static void Main(string[] args)
        {
            AsyncRedisServer server = new();
            server.StartAsync().Wait();
        }
    }
}