using System.Net;
using System.Net.Sockets;
using RedisClone.Services.Serialization;

namespace RedisClone
{
    class Program
    {
        private static Dictionary<string, object> dictionary = new();
        static void Main(string[] args)
        {
            IPAddress address = IPAddress.Any;
            int port = 6379;
            TcpListener listener = new(address, port);
            try
            {
                listener.Start();
                Console.WriteLine($"Server started on port {port}");
                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    Console.WriteLine("Client connected");
                    HandleClient(client);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
            finally
            {
                listener.Stop();
            }
        }

        private static void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            try
            {
                // Receive request
                StreamReader streamReader = new(stream);
                char type = (char)streamReader.Read();
                ISerializationService deSerializationService = SerializationServiceFactory.GetSerializationService(type);
                List<object> commands = (List<object>)deSerializationService.Deserialize(streamReader);
                string command = (string) commands[0];
                object[] args = commands.Skip(1).ToArray();
                
                // Handle request
                object response = null;
                if (command.Equals("PING"))
                {
                    response = "PONG";
                }else if (command.Equals("ECHO"))
                {
                    response = args[0];
                }else if (command.Equals("SET"))
                {
                    string key = (string)args[0];
                    object value = args[1];
                    dictionary[key] = value;
                    response = "OK";
                }else if (command.Equals("GET"))
                {
                    string key = (string)args[0];
                    dictionary.TryGetValue(key, out object value);
                    response = value;
                }
                else
                {
                    response = "";
                }
                // Send response
                ISerializationService serializationService = SerializationServiceFactory.GetSerializationService(response);
                byte[] responseData = serializationService.Serialize(response);
                stream.Write(responseData, 0, responseData.Length);
                Console.WriteLine("Response sent");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
            finally
            {
                stream.Close();
                client.Close();
            }
        }
    }
}