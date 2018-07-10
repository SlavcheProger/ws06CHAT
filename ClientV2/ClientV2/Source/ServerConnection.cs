using System.Net.Sockets;

namespace Chat.Socket.ClientV2.Source
{
    internal class ServerConnection
    {
        private string Ip { get; set; }
        private int Port { get; } = 8888;

        internal static TcpClient client;
        internal static NetworkStream Stream;

        internal ServerConnection(string ip, int port = 8888)
        {
            Ip = ip;
            Port = port;
            client = new TcpClient();
        }

        internal void SetUp()
        {
            client.Connect(Ip, Port);
            Stream = client.GetStream();
        }
    }
}
