using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

public class Server1
{
    // Server - Client Communication using Socket Class , Two-way communication by TURN 

    private const int PORT_NUMBER = 9999;
    public static async Task Main()
    {
        var hostName = Dns.GetHostName();
        IPHostEntry localhost = await Dns.GetHostEntryAsync(hostName);
        IPAddress localIpAddress = localhost.AddressList[0];
        IPEndPoint iPEndPoint = new IPEndPoint(localIpAddress, PORT_NUMBER);
        using Socket listener = new Socket(localIpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        Console.WriteLine("Server started on " + listener.LocalEndPoint);
        Console.WriteLine("Waiting for connection...");


        listener.Bind(iPEndPoint);
        listener.Listen(100);
        var handler = await listener.AcceptAsync();

        Console.WriteLine("Connection received from " + handler.RemoteEndPoint);


        while (true)
        {
            try
            {
                // Receive message.
                var buffer = new byte[1_024];
                var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
                var response = Encoding.UTF8.GetString(buffer, 0, received);

                if (!string.IsNullOrEmpty(response) && response.ToUpper() == "EXIT")
                {
                    Console.WriteLine("Disconnected");
                    break;
                }
                Console.WriteLine($"Client message: \"{response}\"");

                // Send message
                Console.Write("Send Client message: ");
                var messageToClient = Console.ReadLine();
                if (!string.IsNullOrEmpty(messageToClient))
                {
                    var echoBytes = Encoding.UTF8.GetBytes(messageToClient);
                    await handler.SendAsync(echoBytes, 0);
                }
            }
            catch (Exception ex) { Console.WriteLine(ex); }
        }

        listener.Close();
        handler.Close();
    }
}