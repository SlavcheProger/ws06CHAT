using Newtonsoft.Json;

namespace ClientV5.Domains.Messages
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
