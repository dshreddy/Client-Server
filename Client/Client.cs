using System.Net.Sockets;
using System.Text;

namespace Client
{
    class Client
    {
        private static TcpClient client;
        private static NetworkStream stream;

        static void Main(string[] args)
        {
            string serverIp = "192.168.64.9"; 
            int port = 8080;

            try
            {
                client = new TcpClient(serverIp, port);
                stream = client.GetStream();

                Console.WriteLine("Connected to server.");

                Thread readThread = new Thread(ReadMessages);
                readThread.Start();

                while (true)
                {
                    string message = Console.ReadLine();
                    byte[] buffer = Encoding.ASCII.GetBytes(message);
                    stream.Write(buffer, 0, buffer.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private static void ReadMessages()
        {
            byte[] buffer = new byte[1024];
            int byteCount;

            try
            {
                while ((byteCount = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string message = Encoding.ASCII.GetString(buffer, 0, byteCount);
                    Console.WriteLine("Server: " + message);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Disconnected from server.");
            }
        }
    }
}