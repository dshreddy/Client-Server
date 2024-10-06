using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Server
{
    public class Server
    {
        private static List<TcpClient> connectedClients = new List<TcpClient>();
        private static TcpListener listener;

        static void Main(string[] args)
        {
            int port = 8080;
            IPAddress ipAddress = IPAddress.Parse("192.168.64.9"); // Replace with your actual local IP address
            listener = new TcpListener(ipAddress, port);
            listener.Start();

            Console.WriteLine("Server listening on " + ipAddress + ":" + port);

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                connectedClients.Add(client);

                Thread clientThread = new Thread(HandleClient);
                clientThread.Start(client);
            }
        }

        private static void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();
            string clientEndPoint = client.Client.RemoteEndPoint.ToString();

            Console.WriteLine("Client connected: " + clientEndPoint);

            byte[] buffer = new byte[1024];
            int byteCount;

            try
            {
                while ((byteCount = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string message = Encoding.ASCII.GetString(buffer, 0, byteCount);
                    BroadcastMessage(message);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Client disconnected: " + clientEndPoint);
                connectedClients.Remove(client);
            }
        }

        private static void BroadcastMessage(string message)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(message);

            foreach (TcpClient client in connectedClients)
            {
                if (client.Connected)
                {
                    NetworkStream stream = client.GetStream();
                    try
                    {
                        stream.Write(buffer, 0, buffer.Length);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Failed to send message to a client.");
                    }
                }
            }

            Console.WriteLine("Broadcast: " + message);
        }
    }
}
