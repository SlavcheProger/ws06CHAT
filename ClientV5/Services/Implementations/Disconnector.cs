using System;
using System.Net.Sockets;
using System.Threading;
using ClientV5.Services.Interfaces;

namespace ClientV5.Services.Implementations
{
    public class Disconnector: IDisconnector
    {
        private NetworkStream Stream;
        private Thread RecieveThread;
        private Thread CommandHandleThread;
        private TcpClient Client;

        public Disconnector() {}

        void IDisconnector.Configure(NetworkStream stream, Thread recieveThread, Thread commandHandleThread, TcpClient client)
        {
            Stream = stream;
            RecieveThread = recieveThread;
            CommandHandleThread = commandHandleThread;
            Client = client;
        }

        void IDisconnector.Disconnect()
        {
            Stream.Close();
            RecieveThread.Abort();
            CommandHandleThread.Abort();
            Client.Close();
            Environment.Exit(0);
        }
    }
}
