using Newtonsoft.Json;

namespace ClientV5.Domains.Messages
{
    public class ChatMessageFromServerContainer: BaseMessageContainer
    {
        [JsonProperty("Message")]
        public string Text { get; set; }
        public string Name { get; set; }

        public ChatMessageFromServerContainer(string messageText)
        {
            Text = messageText;
            Name = string.Empty;
            Type = MessageType.ChatMessageFromServer;
        }
    }
}
