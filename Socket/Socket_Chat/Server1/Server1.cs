using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

public class Server1
{

    private const int BUFFER_SIZE = 1024;
    private const int PORT_NUMBER = 9999;

    static ASCIIEncoding encoding = new ASCIIEncoding();

    public static void Main()
    {
        try
        {
            IPAddress address = IPAddress.Parse("127.0.0.1");

            TcpListener listener = new TcpListener(address, PORT_NUMBER);

            // 1. listen
            listener.Start();

            Console.WriteLine("Server started on " + listener.LocalEndpoint);
            Console.WriteLine("Waiting for a connection...");

            Socket socket = listener.AcceptSocket();
            Console.WriteLine("Connection received from " + socket.RemoteEndPoint);

            var stream = new NetworkStream(socket);
            var reader = new StreamReader(stream);
            var writer = new StreamWriter(stream);
            while (true)
            {
                try
                {
                    // 2. receive
                    string messageFromClient = reader.ReadLine().Trim();

                    if (string.IsNullOrEmpty(messageFromClient) || messageFromClient.ToUpper() == "EXIT")
                    {
                        writer.WriteLine("BYE");
                        break;
                    }
                    Console.WriteLine("Client: " + messageFromClient);
                }
                catch (IOException e)
                {
                    Console.WriteLine(e);
                }
            }
            stream.Close();
            writer.Close();
            socket.Close();
            listener.Stop();

        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex);
        }
        Console.Read();
    }
}