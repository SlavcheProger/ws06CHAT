using System;
using System.Net.Sockets;

namespace ClientV5.Services.Interfaces
{
    public interface IReciever
    {
        void GettigMessageProcess();

        string GetMessage();

        void ServerMessageHandle(string message);

        void ChatMessageHandle(string jsonString);

        void CommandHandle(string jsonString);

        void ServerNotificationHandle(string jsonString);

        void SetStream(NetworkStream clientStream);
    }
}
