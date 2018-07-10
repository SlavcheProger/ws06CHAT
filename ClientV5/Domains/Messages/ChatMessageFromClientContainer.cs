using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ClientV5.Domains.Messages
{
    public class ChatMessageFromClientContainer: BaseMessageContainer
    {
        [JsonProperty("Message")]
        public string Text { get; set; }
        public JObject Args { get; set; }

        public ChatMessageFromClientContainer()
        {
            Text = string.Empty;
            Args = new JObject();
            Type = MessageType.ChatMessageFromClient;
        }
    }
}
