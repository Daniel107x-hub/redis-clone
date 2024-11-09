namespace RedisClone.Services.Core;

public interface IProcessor
{
    public object? ProcessCommand(string command, List<object> args);
}