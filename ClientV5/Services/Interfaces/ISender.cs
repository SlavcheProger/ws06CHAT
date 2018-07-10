using System.Net.Sockets;
using ClientV5.Domains.Messages;
using Newtonsoft.Json.Linq;

namespace ClientV5.Services.Interfaces
{
    public interface ISender
    {
        void AuthorizeRequest();

        void Pong(JObject Args);

        void SetStream(NetworkStream clientStream);

        void SetUsername(string name);

        void Here();

        void Ping();

        void ChatMessage(string message, string name = null);

        void SendFile(CommandMessageContainer transportContainer, string recipientName = null);

    }
}
