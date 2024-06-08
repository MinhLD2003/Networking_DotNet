using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class Client1
{
    private const int PORT_NUMBER = 9999;

    public Client1()
    {
        try
        {
            // Startup variables
            var hostName = Dns.GetHostName();
            IPHostEntry localhost = Dns.GetHostEntry(hostName);
            IPAddress localIpAddress = localhost.AddressList[0];
            IPEndPoint iPEndPoint = new IPEndPoint(localIpAddress, PORT_NUMBER);
            using Socket client = new Socket(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            client.ConnectAsync(iPEndPoint).Wait();  

            Task sendTask = SendMessage(client);
            Task receiveTask = ReceiveMessage(client);

            Task.WhenAll(sendTask, receiveTask).Wait();  // Wait for both tasks to complete

            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex);
        }
        Console.Read();
    }

    private async Task SendMessage(Socket client)
    {
        try
        {
            while (true)
            {
                Console.Write("[Client] send messages: ");
                string message = Console.ReadLine().Trim();
                if (!string.IsNullOrEmpty(message))
                {
                    var messageBytes = Encoding.UTF8.GetBytes(message);
                    await client.SendAsync(new ArraySegment<byte>(messageBytes), SocketFlags.None);
                    if (message.ToUpper() == "EXIT")
                    {
                        Console.WriteLine("Goodbye...");
                        break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private static async Task ReceiveMessage(Socket client)
    {
        var buffer = new byte[1024];
        try
        {
            while (true)
            {
                var received = await client.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                if (received == 0) break; // Server disconnected

                var response = Encoding.UTF8.GetString(buffer, 0, received);
                Console.WriteLine($"Server message: \"{response}\"");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public static void Main()
    {
        Client1 client = new Client1();
    }
}
