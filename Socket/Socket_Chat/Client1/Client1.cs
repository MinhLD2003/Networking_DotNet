using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;

public class Client1
{

    private const int BUFFER_SIZE = 1024;
    private const int PORT_NUMBER = 9999;

    static ASCIIEncoding encoding = new ASCIIEncoding();

    public static void Main()
    {

        try
        {
            TcpClient client = new TcpClient();

            // 1. connect
            client.Connect("127.0.0.1", PORT_NUMBER);
            Stream stream = client.GetStream();

            Console.WriteLine("Connected to Server.");
            var reader = new StreamReader(stream);
            var writer = new StreamWriter(stream);
            writer.AutoFlush = true;
            while (true)
            {
                try
                {
                    Console.Write("Message to Server: ");
                    string messageToServer = Console.ReadLine().Trim();
                    if (!string.IsNullOrEmpty(messageToServer))
                    {
                        writer.WriteLine(messageToServer);
                    }

                }
                catch (IOException e)
                {
                    Console.WriteLine("Ex" + e);
                }
            }
            stream.Close();
            client.Close();
        }

        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex);
        }

        Console.Read();
    }
}