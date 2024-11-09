using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using RedisClone.Services.Core;
using RedisClone.Services.Serialization;

namespace RedisClone;

public class AsyncRedisServer
{
    private readonly IProcessor _commandProcessor;
    private readonly TcpListener _tcpListener;
    private readonly ILogger _logger;
    private const int Port = 6379;
    
    public AsyncRedisServer(IProcessor processor)
    {
        _commandProcessor = processor;
        _tcpListener = new TcpListener(IPAddress.Any, Port);
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
        List<object> args = commands.Skip(1).ToList();
        var response = _commandProcessor.ProcessCommand(command, args);
        ISerializationService serializationService = SerializationServiceFactory.GetSerializationService(response);
        return serializationService.Serialize(response);
    }
}