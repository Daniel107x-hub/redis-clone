using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using RedisClone.Services.Serialization;

namespace RedisClone;

public class AsyncRedisServer
{
    private Dictionary<string, object> _dictionary;
    private readonly TcpListener _tcpListener;
    private const int Port = 6379;
    private int _clients;
    private int _totalSetRequests;
    private int _totalGetRequests;
    private int _totalConfigRequests;
    private int _totalUnrecognizedRequests;
    private ILogger _logger;
    
    public AsyncRedisServer()
    {
        _tcpListener = new TcpListener(IPAddress.Any, Port);
        _dictionary = new();
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        _logger = factory.CreateLogger("Redis Lite Server");
        _logger.LogInformation("Logging is set up and ready");
    }

    public async Task StartAsync()
    {
        try
        {
            _tcpListener.Start();
            _logger.LogInformation("Server started");
            while (true)
            {
                TcpClient client = await _tcpListener.AcceptTcpClientAsync();
                _logger.LogTrace("Client connected");
                _clients++;
                _ = HandleClientAsync(client);
            }
        }
        catch (Exception e)
        {
            _logger.LogError($"Error: {e.Message}");
        }
        finally
        {
            _tcpListener.Stop();
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        using NetworkStream stream = client.GetStream();
        try
        {
            // Receive request
            StreamReader streamReader = new(stream);
            while (streamReader.Peek() != -1)
            {
                // Process request
                _logger.LogTrace("New request received");
                byte[] response = await Task.Run(() => ProcessRequest(streamReader));
                // Send response
                stream.Write(response, 0, response.Length);
                _logger.LogTrace("Response sent");
            }
            _logger.LogInformation(PrintMetrics());
        }
        catch (Exception e)
        {
            _logger.LogError($"Error: {e.Message}");
        }
        finally
        {
            stream.Close();
            client.Close();
        }
    }

    private byte[] ProcessRequest(StreamReader streamReader)
    {
        char type = (char)streamReader.Read();
        ISerializationService deSerializationService = SerializationServiceFactory.GetSerializationService(type);
        List<object> commands = (List<object>)deSerializationService.Deserialize(streamReader);
        string command = (string) commands[0];
        object[] args = commands.Skip(1).ToArray();
            
        // Handle request
        object response;
        if (command.Equals("PING"))
        {
            response = "PONG";
        }else if (command.Equals("ECHO"))
        {
            response = args[0];
        }else if (command.Equals("SET"))
        {
            Interlocked.Increment(ref _totalSetRequests);
            string key = (string)args[0];
            object value = args[1];
            _dictionary[key] = value;
            response = "OK";
        }else if (command.Equals("GET"))
        {
            Interlocked.Increment(ref _totalGetRequests);
            string key = (string)args[0];
            _dictionary.TryGetValue(key, out object value);
            response = value;
        }else if (command.Equals("RESET"))
        {
            _clients = 0;
            _dictionary.Clear();
            Interlocked.Exchange(ref _totalSetRequests, 0);
            Interlocked.Exchange(ref _totalGetRequests, 0);
            Interlocked.Exchange(ref _totalConfigRequests, 0);
            Interlocked.Exchange(ref _totalUnrecognizedRequests, 0);
            response = "OK";
        }
        else
        {
            if(command.Equals("CONFIG")) Interlocked.Increment(ref _totalConfigRequests);
            else Interlocked.Increment(ref _totalUnrecognizedRequests);
            response = "";
        }
        ISerializationService serializationService = SerializationServiceFactory.GetSerializationService(response);
        return serializationService.Serialize(response);
    }
    
    private string PrintMetrics()
    {
        return $"Clients: {_clients}, Total SET requests: {_totalSetRequests}, Total GET requests: {_totalGetRequests}, Total CONFIG requests: {_totalConfigRequests}, Total unrecognized requests: {_totalUnrecognizedRequests}";
    }
}