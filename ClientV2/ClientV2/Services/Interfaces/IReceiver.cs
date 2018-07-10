using Chat.Socket.ClientV2.MessageTypes;
using System.Threading;

namespace Chat.Socket.ClientV2.Services.Interfaces
{
    interface IReceiver
    {
        Thread Thread { get; set; }

        void ReceiveMessage();

        void Sorter(string message);

        void NotificationHandler(string message);

        void ChatMessageHandler(string message);

        void CommandSorter(string message);

        void Start(CommandMessage command);
        
        void SetUserName(CommandMessage command);

        void Here(CommandMessage command);

        void Ping(CommandMessage command);

        void SendFile(CommandMessage command);

        void Pong(CommandMessage command);

        void AbortThread();
    }
}
