using System;
using System.Net.Sockets;
using System.Text;
using Chat.Socket.Server.Services;
using ClientV5.Domains.Messages;
using ClientV5.Services.Interfaces;
using Newtonsoft.Json;

namespace ClientV5.Services.Implementations
{
    public class Reciever : IReciever
    {
        private NetworkStream stream;

        private readonly IReciever self;
        private readonly ILogger logger;
        private readonly IChatCore chatCore;

        public Reciever()
        {
            logger = DependencyResolver.Get<ILogger>();
            chatCore = DependencyResolver.Get<IChatCore>();

            self = this as IReciever;
        }

        void IReciever.SetStream(NetworkStream clientStream)
        {
            stream = clientStream;
        }

        void IReciever.GettigMessageProcess()
        {
            while (true)
            {
                try
                {
                    self.ServerMessageHandle(self.GetMessage());
                }
                catch (Exception exception)
                {
                    logger.Write(exception);
                    break;
                }
            }
        }

        string IReciever.GetMessage()
        {
            var data = new byte[2097152];
            var builder = new StringBuilder();
            var bytes = 0;

            do
            {
                bytes = stream.Read(data, 0, data.Length);
                builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
            }
            while (stream.DataAvailable);

            var messageJsonString = builder.ToString();

            return messageJsonString;
        }

        void IReciever.ServerMessageHandle(string message)
        {
            var messageType = JsonConvert.DeserializeObject<BaseMessageContainer>(message).Type;

            switch (messageType)
            {
                case MessageType.ChatMessageFromServer:
                    self.ChatMessageHandle(message);
                    break;

                case MessageType.CommandMessage:
                    self.CommandHandle(message);
                    break;

                case MessageType.ServerNotification:
                    self.ServerNotificationHandle(message);
                    break;

                default:
                    break;
            }
        }

        void IReciever.ChatMessageHandle(string jsonString)
        {
            var chatMessage = JsonConvert.DeserializeObject<ChatMessageFromServerContainer>(jsonString);
            chatCore.ChatMessageHandle(chatMessage);
        }

        void IReciever.CommandHandle(string jsonString)
        {
            var commandMessage = JsonConvert.DeserializeObject<CommandMessageContainer>(jsonString);
            chatCore.CommandHandle(commandMessage);
        }

        void IReciever.ServerNotificationHandle(string jsonString)
        {
            var serverNotification = JsonConvert.DeserializeObject<ServerNotificationContainer>(jsonString);
            chatCore.ServerNotificationHandle(serverNotification);
        }
    }
}
