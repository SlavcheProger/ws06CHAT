using System;
using System.Net;
using System.Net.Sockets;

namespace Client1
{
    class ServerConnection
    {
        private string ip;
        private int port = 8888;

        internal TcpClient client;
        internal NetworkStream stream;

        internal ServerConnection()
        {
            var display = new DisplayMessageService();
            ip = ReadIp();
            ip = ip ?? "192.168.1.135";
            client = new TcpClient();
            display.Display($"Connecting to {ip}:{port}", DisplayMessageType.System);
            client.Connect(ip, port);
            stream = client.GetStream();
        }
        private string ReadIp()
        {
            var display = new DisplayMessageService();
            display.Display("Write ip to start connecting to it.", DisplayMessageType.System);

            var result = string.Empty;
            while (true)
            {
                result = Console.ReadLine().Trim();

                IPAddress ipAddress;
                if (IPAddress.TryParse(result, out ipAddress) || result == "localhost")
                {
                    break;
                }

                if (string.IsNullOrEmpty(result))
                {
                    result = null;
                    break;
                }

                display.Display("Please, write correct ip.", DisplayMessageType.Error);
            }

            return result;
        }
        internal void CloseConnection()
        {
            stream?.Close();
            client?.Close();
        }
    }
}
