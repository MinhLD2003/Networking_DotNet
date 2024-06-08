using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

public class Server1
{
    private const int PORT_NUMBER = 9999;
    private Socket listener = null;
    private IPEndPoint iPEndPoint = null;
    private Thread listenThread = null;

    public Server1()
    {
        try
        {
           
            var hostName = Dns.GetHostName();
            IPHostEntry localhost = Dns.GetHostEntry(hostName);
            IPAddress localIpAddress = localhost.AddressList[0];
            iPEndPoint = new IPEndPoint(localIpAddress, PORT_NUMBER);
            listener = new Socket(localIpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(iPEndPoint);
            listener.Listen(100);
            Console.WriteLine("Server started on " + listener.LocalEndPoint);
            Console.WriteLine("Waiting for connection...");
            Task.Run(() => ListenForClients());
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private async Task ListenForClients()
    {
        while (true)
        {
            try
            {
                var handler = listener.Accept();
                Console.WriteLine("Connection received from " + handler.RemoteEndPoint);
                await Task.Run(() => HandleClientMessage(handler));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }

    private async Task HandleClientMessage(Socket handler)
    {
        var buffer = new byte[1024];
        try
        {
            while (true)
            {
                var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
                if (received == 0) break; 

                var response = Encoding.UTF8.GetString(buffer, 0, received);
                Console.WriteLine($"Client message: \"{response}\"");

                if (response.ToUpper() == "EXIT")
                {
                    Console.WriteLine("Client disconnected");
                    break;
                }
                await handler.SendAsync(new ArraySegment<byte>(buffer, 0, received), SocketFlags.None);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }
    }

    public static async Task Main()
    {
        Server1 server = new Server1();
        await Task.Delay(-1); 
    }
}
