using System.Collections.Concurrent;

namespace RedisClone.Services.Core;

public class RedisCommandProcessor : IProcessor
{
    private readonly ConcurrentDictionary<string, object> _dictionary = new();
    private readonly ConcurrentDictionary<string, long> _expirationDictionary = new();
    
    public object? ProcessCommand(string command, List<object> args)
    {
        // Handle request
        switch (command)
        {
            case "PING":
                return "PONG";
            
            case "ECHO":
                if(args.Count < 1) return new ArgumentException("ECHO command requires AT LEAST 1 argument");
                return args[0];
            
            case "SET":
                if(args.Count < 2) return new ArgumentException("SET command requires AT LEAST 2 arguments");
                var key = (string)args[0];
                var value = args[1];
                _dictionary[key] = value;
                var additionalArguments = GetArgsDictionary(args.Skip(2).ToList());
                if (additionalArguments.Count == 0) return "OK";
                if (additionalArguments.TryGetValue("EXP", out var offsetObject))
                {
                    var success = long.TryParse(offsetObject.ToString(), out var offset);
                    if (!success) return new ArgumentException("Invalid value for EXP argument");
                    var timeSpan = TimeSpan.FromSeconds(offset);
                    _expirationDictionary[key] = DateTime.Now.Add(timeSpan).Ticks;
                }
                else if (additionalArguments.TryGetValue("PX", out offsetObject))
                {
                    var success = long.TryParse(offsetObject.ToString(), out var offset);
                    if (!success) return new ArgumentException("Invalid value for PX argument");
                    var timeSpan = TimeSpan.FromMilliseconds(offset);
                    _expirationDictionary[key] = DateTime.Now.Add(timeSpan).Ticks;
                }
                else if (additionalArguments.TryGetValue("EXAT", out offsetObject))
                {
                    var success = long.TryParse(offsetObject.ToString(), out var timestamp);
                    if (!success) return new ArgumentException("Invalid value for EXAT argument");
                    DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timestamp);
                    var dateTime = dateTimeOffset.LocalDateTime;
                    _expirationDictionary[key] = dateTime.Ticks;
                }
                else if (additionalArguments.TryGetValue("PXAT", out offsetObject))
                {
                    var success = long.TryParse(offsetObject.ToString(), out var timestamp);
                    if (!success) return new ArgumentException("Invalid value for PXAT argument");
                    DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(timestamp);
                    var dateTime = dateTimeOffset.LocalDateTime;
                    _expirationDictionary[key] = dateTime.Ticks;
                }
                return "OK";

            case "GET":
                if(args.Count < 1) return new ArgumentException("GET command requires AT LEAST 1 argument");
                key = (string)args[0];
                if(_expirationDictionary.TryGetValue(key, out long expiration))
                {
                    if(DateTime.Now.Ticks > expiration)
                    {
                        _dictionary.TryRemove(key, out _);
                        _expirationDictionary.TryRemove(key, out _);
                        return null;
                    }
                }
                _dictionary.TryGetValue(key, out value);
                return value;
            
            default:
                return null;
        }
    }
    
    private Dictionary<string, object> GetArgsDictionary(List<object> args)
    {
        if(args.Count % 2 != 0) throw new ArgumentException("Invalid number of arguments");
        Dictionary<string, object> dictionary = new();
        for (int i = 0; i < args.Count(); i += 2)
        {
            dictionary.Add((string)args.ElementAt(i), (string)args.ElementAt(i + 1));
        }
        return dictionary;
    }
}