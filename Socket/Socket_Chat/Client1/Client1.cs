using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;

public class Client1
{
    // Server - Client Communication using Socket Class
    private const int PORT_NUMBER = 9999;
    public static async Task Main()
    {
        try
        {
            // Startup variables
            var hostName = Dns.GetHostName();
            IPHostEntry localhost = await Dns.GetHostEntryAsync(hostName);
            IPAddress localIpAddress = localhost.AddressList[0];
            IPEndPoint iPEndPoint = new IPEndPoint(localIpAddress, PORT_NUMBER);
            using Socket client = new Socket(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            await client.ConnectAsync(iPEndPoint);

            while (true)
            {
                try
                {
                    Console.Write("Send Server message: ");
                    var message = Console.ReadLine().Trim();
                    if (string.IsNullOrEmpty(message))
                    {
                        Console.WriteLine("Please enter messages");
                        continue;
                    }
                    var messageBytes = Encoding.UTF8.GetBytes(message);
                    await client.SendAsync(messageBytes, SocketFlags.None);
                    if (message.ToUpper() == "EXIT")
                    {
                        Console.WriteLine("Goodbye!!!...");
                        Console.WriteLine("Disconnected...");
                        break;
                    }
                    // Receive ack.
                    var buffer = new byte[1_024];
                    var receivedMessage = await client.ReceiveAsync(buffer, SocketFlags.None);
                    var response = Encoding.UTF8.GetString(buffer, 0, receivedMessage);

                    Console.WriteLine(
                        $"Server message: \"{response}\"");

                }
                catch (IOException e)
                {
                    Console.WriteLine("Ex" + e);
                }
            }
            client.Shutdown(SocketShutdown.Both);

        }

        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex);
        }

        Console.Read();
    }
}