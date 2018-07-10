using Newtonsoft.Json;

namespace Chat.Socket.Server.Domains.Messages
{
    public class BaseMessageContainer
    {
        [JsonProperty("MessageType")]
        public MessageType Type { get; set; }

        public BaseMessageContainer()
        {
            Type = MessageType.ServerNotification;
        }
    }
}
