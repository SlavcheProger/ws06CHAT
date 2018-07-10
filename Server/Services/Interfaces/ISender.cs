using Chat.Socket.Server.Domains.Messages;
using Newtonsoft.Json.Linq;

namespace Chat.Socket.Server.Services.Interfaces
{
    public interface ISender
    {
        bool RecipientIsValid(JObject Args);

        void Broadcast(ClientConnection connection, string text, JObject Args);

        void ServerNotification(ClientConnection connection, string notificationText);

        void ServerNotificationToAll(string notificationText, ClientConnection connection = null);

        void StartCallback(ClientConnection connection);

        void HereCallback(ClientConnection connection, string[] usersNames);

        void Ping(ClientConnection connection, int ID);

        void Pong(ClientConnection connection, int ID);

        void SetUsernameCallback(ClientConnection connection, string username);

        void MessageToRecipient(string recipientName, ChatMessageFromServerContainer transportContainer);

        void MessageToAll(ClientConnection connection, ChatMessageFromServerContainer transportContainer);

        void BroadcastFile(ClientConnection connection, JObject Args);

        void FileToAll(ClientConnection connection, CommandMessageContainer transportContainer);

        void FileToRecipient(string recipientName, CommandMessageContainer transportContainer);
    }
}
