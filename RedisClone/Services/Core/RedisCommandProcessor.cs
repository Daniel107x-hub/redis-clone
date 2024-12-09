using System.Collections.Concurrent;
using RedisClone.Services.Time;

namespace RedisClone.Services.Core;

public class RedisCommandProcessor : IProcessor
{
    private readonly ConcurrentDictionary<string, object> _dictionary = new();
    private readonly ConcurrentDictionary<string, long> _expirationDictionary = new();
    private readonly ITimeProvider _timeProvider;
    
    public RedisCommandProcessor(ITimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }
    
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
                    _expirationDictionary[key] = _timeProvider.Now.Add(timeSpan).Ticks;
                }
                else if (additionalArguments.TryGetValue("PX", out offsetObject))
                {
                    var success = long.TryParse(offsetObject.ToString(), out var offset);
                    if (!success) return new ArgumentException("Invalid value for PX argument");
                    var timeSpan = TimeSpan.FromMilliseconds(offset);
                    _expirationDictionary[key] = _timeProvider.Now.Add(timeSpan).Ticks;
                }
                else if (additionalArguments.TryGetValue("EXAT", out offsetObject))
                {
                    var success = long.TryParse(offsetObject.ToString(), out var timestamp);
                    if (!success) return new ArgumentException("Invalid value for EXAT argument");
                    DateTimeOffset dateTimeOffset = _timeProvider.FromUnixTimeSeconds(timestamp);
                    var dateTime = dateTimeOffset.LocalDateTime;
                    _expirationDictionary[key] = dateTime.Ticks;
                }
                else if (additionalArguments.TryGetValue("PXAT", out offsetObject))
                {
                    var success = long.TryParse(offsetObject.ToString(), out var timestamp);
                    if (!success) return new ArgumentException("Invalid value for PXAT argument");
                    DateTimeOffset dateTimeOffset = _timeProvider.FromUnixTimeMilliseconds(timestamp);
                    var dateTime = dateTimeOffset.LocalDateTime;
                    _expirationDictionary[key] = dateTime.Ticks;
                }
                return "OK";

            case "GET":
                if(args.Count < 1) return new ArgumentException("GET command requires AT LEAST 1 argument");
                key = (string)args[0];
                if(_expirationDictionary.TryGetValue(key, out long expiration))
                {
                    if(_timeProvider.Now.Ticks > expiration)
                    {
                        _dictionary.TryRemove(key, out _);
                        _expirationDictionary.TryRemove(key, out _);
                        return null;
                    }
                }
                _dictionary.TryGetValue(key, out value);
                return value;
            
            case "EXISTS":
                if(args.Count < 1) return new ArgumentException("EXISTS command requires AT LEAST 1 argument");
                int count = 0;
                for (int i = 0; i < args.Count; i++)
                {
                    key = (string)args[i];
                    if(_dictionary.ContainsKey(key)) count++;
                }

                return count;
            
            case "DEL":
                if(args.Count < 1) return new ArgumentException("DEL command requires AT LEAST 1 argument");
                count = 0;
                for(int i = 0; i < args.Count; i++)
                {
                    key = (string)args[i];
                    if(_dictionary.TryRemove(key, out _))
                    {
                        _expirationDictionary.TryRemove(key, out _);
                        count++;
                    }
                }
                return count;
            
            case "INCR":
                if(args.Count < 1) return new ArgumentException("INCR command requires AT LEAST 1 argument");
                key = (string)args[0];
                if (!_dictionary.ContainsKey(key))
                {
                    _dictionary.TryAdd(key, "0");
                    return 0;
                }
                var result = int.TryParse(_dictionary[key].ToString(), out count);
                if(!result) return new ArgumentException("Invalid value for INCR argument");
                _dictionary[key] = (count++).ToString();
                return count;
            
            case "DECR": 
                if(args.Count < 1) return new ArgumentException("INCR command requires AT LEAST 1 argument");
                key = (string)args[0];
                if (!_dictionary.ContainsKey(key))
                {
                    _dictionary.TryAdd(key, "0");
                    return 0;
                }
                result = int.TryParse(_dictionary[key].ToString(), out count);
                if(!result) return new ArgumentException("Invalid value for DECR argument");
                _dictionary[key] = (count--).ToString();
                return count;
            
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