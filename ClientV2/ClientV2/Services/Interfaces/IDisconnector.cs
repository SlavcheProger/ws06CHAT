using System.Net.Sockets;
using System.Threading;

namespace Chat.Socket.ClientV2.Services.Interfaces
{
    interface IDisconnector
    {
        void Configure(NetworkStream stream, TcpClient tcpClient, Thread rThread, Thread cThread);

        void Disconnect();
    }
}

