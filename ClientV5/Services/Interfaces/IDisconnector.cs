using System;
using System.Net.Sockets;
using System.Threading;

namespace ClientV5.Services.Interfaces
{
    public interface IDisconnector
    {
        void Configure(NetworkStream stream, Thread recieveThread, Thread commandHandleThread, TcpClient client);
        void Disconnect();

    }
}
