using System;
using ClientV5.Domains.Messages;

namespace ClientV5.Services.Interfaces
{
    public interface IChatCore
    {
        void CommandHandle(CommandMessageContainer commandMessage);

        void ChatMessageHandle(ChatMessageFromServerContainer chatMessage);

        void ServerNotificationHandle(ServerNotificationContainer serverNotification);
    }
}
