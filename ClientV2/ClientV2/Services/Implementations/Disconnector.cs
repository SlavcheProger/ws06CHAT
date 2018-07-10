using Chat.Socket.ClientV2.Services.Interfaces;
using System;
using System.Net.Sockets;
using System.Threading;

namespace Chat.Socket.ClientV2.Services.Implementations
{
    class Disconnector : IDisconnector
    {
        private NetworkStream stream;
        private TcpClient client;
        private Thread receiveThread;
        private Thread commandThread;
        

        public Disconnector()
        {}

        void IDisconnector.Configure(NetworkStream clientStream, TcpClient tcpClient,Thread rThread , Thread cThread)
        {
            stream = clientStream;
            client = tcpClient;
            receiveThread = rThread;
            commandThread = cThread;
        }

        void IDisconnector.Disconnect()
        {
            receiveThread.Abort();
            commandThread.Abort();

            client?.Close();
            stream?.Close();

            Environment.Exit(0);
        }
    }
}
